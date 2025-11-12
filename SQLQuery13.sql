CREATE OR ALTER PROCEDURE [dbo].[sp_UpdateUnavailableDates]
    @Name NVARCHAR(100),
    @NewDateRange NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OldDateRanges NVARCHAR(MAX), 
            @OldDateRangesS NVARCHAR(MAX),
            @Today DATE = GETDATE(),
            @TwoMonthsAgo DATE = DATEADD(MONTH, -2, GETDATE());

    -- Fetch existing data
    SELECT 
        @OldDateRanges = DateRanges,
        @OldDateRangesS = DateRangesS
    FROM BookingTable
    WHERE Name = @Name;

    -- Ensure NULLs become empty strings
    SET @OldDateRanges = ISNULL(@OldDateRanges, '');
    SET @OldDateRangesS = ISNULL(@OldDateRangesS, '');

    -- Step 1: Append the new date to DateRangesS (history)
    SET @OldDateRangesS =
        CASE 
            WHEN @OldDateRangesS = '' THEN @NewDateRange
            ELSE @OldDateRangesS + '|' + @NewDateRange
        END;

    -- Step 2: Clean DateRangesS (remove older than 2 months)
    DECLARE @CleanedDateRangesS NVARCHAR(MAX);
    ;WITH SplitDates AS (
        SELECT TRIM(value) AS RangeText
        FROM STRING_SPLIT(@OldDateRangesS, '|')
        WHERE TRIM(value) <> ''
    )
    SELECT @CleanedDateRangesS = STRING_AGG(RangeText, '|')
    FROM SplitDates
    WHERE TRY_CONVERT(DATE, LEFT(RangeText, 10)) >= @TwoMonthsAgo;

    -- Step 3: Create updated DateRanges (keep today and future only)
    DECLARE @UpdatedDateRanges NVARCHAR(MAX);
    ;WITH SplitDates AS (
        SELECT TRIM(value) AS RangeText
        FROM STRING_SPLIT(@CleanedDateRangesS, '|')
        WHERE TRIM(value) <> ''
    )
    SELECT @UpdatedDateRanges = STRING_AGG(RangeText, '|')
    FROM SplitDates
    WHERE TRY_CONVERT(DATE, LEFT(RangeText, 10)) >= @Today;

    -- Step 4: Make sure the new date is in both
    IF CHARINDEX(@NewDateRange, @UpdatedDateRanges) = 0
        SET @UpdatedDateRanges =
            CASE 
                WHEN @UpdatedDateRanges IS NULL OR @UpdatedDateRanges = '' 
                THEN @NewDateRange
                ELSE @UpdatedDateRanges + '|' + @NewDateRange
            END;

    IF CHARINDEX(@NewDateRange, @CleanedDateRangesS) = 0
        SET @CleanedDateRangesS =
            CASE 
                WHEN @CleanedDateRangesS IS NULL OR @CleanedDateRangesS = '' 
                THEN @NewDateRange
                ELSE @CleanedDateRangesS + '|' + @NewDateRange
            END;

    -- Step 5: Update the table
    UPDATE BookingTable
    SET 
        DateRanges = @UpdatedDateRanges,
        DateRangesS = @CleanedDateRangesS
    WHERE Name = @Name;

    -- Step 6: Return the updated record
    SELECT BookingID, Name, DateRanges, DateRangesS
    FROM BookingTable
    WHERE Name = @Name;
END;
GO
