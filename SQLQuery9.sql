CREATE OR ALTER PROCEDURE UpdateBookingDates_KeepFuture
    @Name NVARCHAR(100),
    @NewDateRanges NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OldDateRanges NVARCHAR(MAX);
    DECLARE @CurrentDate DATE = GETDATE();
    DECLARE @FutureRanges NVARCHAR(MAX) = '';
    DECLARE @PastRanges NVARCHAR(MAX) = '';

    ------------------------------------------------------------
    -- Step 1: Fetch old data
    ------------------------------------------------------------
    SELECT @OldDateRanges = DateRanges
    FROM BookingTable
    WHERE Name = @Name;

    IF @OldDateRanges IS NULL
    BEGIN
        SELECT '❌ No record found for this name' AS Message;
        RETURN;
    END

    ------------------------------------------------------------
    -- Step 2: Split existing date ranges into past & future
    ------------------------------------------------------------
    DECLARE @XML XML = CAST('<r><d>' + REPLACE(@OldDateRanges, '|', '</d><d>') + '</d></r>' AS XML);
    DECLARE @StartDate DATE, @EndDate DATE;

    DECLARE cur CURSOR FOR
        SELECT 
            TRY_CONVERT(DATE, LEFT(T.split.value('.', 'NVARCHAR(MAX)'), CHARINDEX('~', T.split.value('.', 'NVARCHAR(MAX)')) - 1)) AS StartDate,
            TRY_CONVERT(DATE, SUBSTRING(T.split.value('.', 'NVARCHAR(MAX)'), CHARINDEX('~', T.split.value('.', 'NVARCHAR(MAX)')) + 1, 20)) AS EndDate
        FROM @XML.nodes('/r/d') AS T(split);

    OPEN cur;
    FETCH NEXT FROM cur INTO @StartDate, @EndDate;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @EndDate < @CurrentDate
        BEGIN
            -- Old date range → history
            SET @PastRanges = 
                CASE 
                    WHEN LEN(@PastRanges) = 0 THEN 
                        CONCAT(CONVERT(VARCHAR(10), @StartDate, 23), '~', CONVERT(VARCHAR(10), @EndDate, 23))
                    ELSE 
                        CONCAT(@PastRanges, '|', CONVERT(VARCHAR(10), @StartDate, 23), '~', CONVERT(VARCHAR(10), @EndDate, 23))
                END;
        END
        ELSE
        BEGIN
            -- Still future → keep in main
            SET @FutureRanges = 
                CASE 
                    WHEN LEN(@FutureRanges) = 0 THEN 
                        CONCAT(CONVERT(VARCHAR(10), @StartDate, 23), '~', CONVERT(VARCHAR(10), @EndDate, 23))
                    ELSE 
                        CONCAT(@FutureRanges, '|', CONVERT(VARCHAR(10), @StartDate, 23), '~', CONVERT(VARCHAR(10), @EndDate, 23))
                END;
        END

        FETCH NEXT FROM cur INTO @StartDate, @EndDate;
    END

    CLOSE cur;
    DEALLOCATE cur;

    ------------------------------------------------------------
    -- Step 3: Merge new future ranges with remaining future ones
    ------------------------------------------------------------
    IF LEN(@FutureRanges) > 0
        SET @FutureRanges = CONCAT(@FutureRanges, '|', @NewDateRanges);
    ELSE
        SET @FutureRanges = @NewDateRanges;

    ------------------------------------------------------------
    -- Step 4: Clean history older than 2 months (optional)
    ------------------------------------------------------------
    DECLARE @CleanHistory NVARCHAR(MAX) = '';
    IF LEN(@PastRanges) > 0
    BEGIN
        DECLARE @CutoffDate DATE = DATEADD(MONTH, -2, @CurrentDate);
        DECLARE @XML2 XML = CAST('<r><d>' + REPLACE(@PastRanges, '|', '</d><d>') + '</d></r>' AS XML);
        DECLARE @S DATE, @E DATE;

        DECLARE cur2 CURSOR FOR
            SELECT 
                TRY_CONVERT(DATE, LEFT(T.split.value('.', 'NVARCHAR(MAX)'), CHARINDEX('~', T.split.value('.', 'NVARCHAR(MAX)')) - 1)),
                TRY_CONVERT(DATE, SUBSTRING(T.split.value('.', 'NVARCHAR(MAX)'), CHARINDEX('~', T.split.value('.', 'NVARCHAR(MAX)')) + 1, 20))
            FROM @XML2.nodes('/r/d') AS T(split);

        OPEN cur2;
        FETCH NEXT FROM cur2 INTO @S, @E;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            IF @E >= @CutoffDate
            BEGIN
                SET @CleanHistory = 
                    CASE 
                        WHEN LEN(@CleanHistory) = 0 THEN 
                            CONCAT(CONVERT(VARCHAR(10), @S, 23), '~', CONVERT(VARCHAR(10), @E, 23))
                        ELSE 
                            CONCAT(@CleanHistory, '|', CONVERT(VARCHAR(10), @S, 23), '~', CONVERT(VARCHAR(10), @E, 23))
                    END;
            END
            FETCH NEXT FROM cur2 INTO @S, @E;
        END

        CLOSE cur2;
        DEALLOCATE cur2;
    END

    ------------------------------------------------------------
    -- Step 5: Update table
    ------------------------------------------------------------
    UPDATE BookingTable
    SET DateRanges = @FutureRanges,      -- Keep current + new future dates
        DateRangesS = @CleanHistory      -- Store last 2 months of past dates
    WHERE Name = @Name;

    ------------------------------------------------------------
    -- Step 6: Return output
    ------------------------------------------------------------
    SELECT 
        '✅ Future dates updated, past moved to history' AS Message,
        @FutureRanges AS ActiveFutureDates,
        @CleanHistory AS HistoryDates;
END;
