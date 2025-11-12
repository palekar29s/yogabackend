CREATE PROCEDURE UpdateBookingDates
    @Name NVARCHAR(100),
    @NewDateRanges NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OldDateRanges NVARCHAR(MAX);

    -- Get the old value
    SELECT @OldDateRanges = DateRanges
    FROM BookingTable
    WHERE Name = @Name;

    -- If record found
    IF @OldDateRanges IS NOT NULL
    BEGIN
        UPDATE BookingTable
        SET DateRangesS = @OldDateRanges,   -- store old in DateRangesS
            DateRanges = @NewDateRanges     -- update with new date
        WHERE Name = @Name;

        SELECT 'Updated Successfully' AS Message;
    END
    ELSE
    BEGIN
        SELECT 'No record found for this name' AS Message;
    END
END;
