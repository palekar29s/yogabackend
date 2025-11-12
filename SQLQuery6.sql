CREATE OR ALTER PROCEDURE UpdateBookingDates_CleanHistory
    @Name NVARCHAR(100),
    @NewDateRanges NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OldDateRanges NVARCHAR(MAX);
    DECLARE @FilteredHistory NVARCHAR(MAX) = '';

    -- Step 1: Fetch old DateRanges
    SELECT @OldDateRanges = DateRanges
    FROM BookingTable
    WHERE Name = @Name;

    IF @OldDateRanges IS NULL
    BEGIN
        SELECT '❌ No record found for this name' AS Message;
        RETURN;
    END

    ------------------------------------------------------------
    -- Step 2: Filter only date ranges newer than 2 months ago
    ------------------------------------------------------------
    DECLARE @XML XML;
    SET @XML = CAST('<r><d>' + REPLACE(@OldDateRanges, '|', '</d><d>') + '</d></r>' AS XML);

    DECLARE @CurrentDate DATE = GETDATE();
    DECLARE @CutoffDate DATE = DATEADD(MONTH, -2, @CurrentDate);

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
        IF @EndDate >= @CutoffDate  -- Keep only ranges within last 2 months
        BEGIN
            SET @FilteredHistory = 
                CASE 
                    WHEN LEN(@FilteredHistory) = 0 THEN 
                        CONCAT(CONVERT(VARCHAR(10), @StartDate, 23), '~', CONVERT(VARCHAR(10), @EndDate, 23))
                    ELSE 
                        CONCAT(@FilteredHistory, '|', CONVERT(VARCHAR(10), @StartDate, 23), '~', CONVERT(VARCHAR(10), @EndDate, 23))
                END;
        END
        FETCH NEXT FROM cur INTO @StartDate, @EndDate;
    END

    CLOSE cur;
    DEALLOCATE cur;

    ------------------------------------------------------------
    -- Step 3: Update DateRanges and DateRangesS
    ------------------------------------------------------------
    UPDATE BookingTable
    SET DateRangesS = @FilteredHistory,       -- Only last 2 months of history
        DateRanges = @NewDateRanges           -- Add new updated value
    WHERE Name = @Name;

    ------------------------------------------------------------
    -- Step 4: Return a message
    ------------------------------------------------------------
    SELECT 
        '✅ Updated and cleaned history successfully' AS Message,
        @FilteredHistory AS SavedHistory,
        @NewDateRanges AS CurrentDateRanges;
END;
