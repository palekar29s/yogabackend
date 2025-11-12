ALTER TABLE [yoga].[dbo].[BookingTable]
ADD DateRangesS NVARCHAR(MAX);
UPDATE [yoga].[dbo].[BookingTable]
SET DateRangesS = DateRanges;