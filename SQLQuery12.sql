SELECT TOP (1000) [BookingID]
      ,[Name]
      ,[DateRanges]
      ,[DateRangesS]
  FROM [yoga ].[dbo].[BookingTable]
  UPDATE [yoga].[dbo].[BookingTable]
SET 
    [DateRanges] = [DateRanges] + '|2025-06-15~2025-06-17|2025-07-05~2025-07-06',
    [DateRangesS] = [DateRangesS] + '|2025-06-15~2025-06-17|2025-07-05~2025-07-06'
WHERE [Name] = 'Rohit Mehta';
UPDATE [yoga].[dbo].[BookingTable]
SET 
    [DateRanges] = 
        '2025-06-10~2025-06-12|2025-07-05~2025-07-07|2025-09-01~2025-09-03|2025-10-10~2025-10-12|2025-11-20~2025-11-22|2025-12-05~2025-12-07',
    [DateRangesS] = 
        '2025-06-10~2025-06-12|2025-07-05~2025-07-07|2025-09-01~2025-09-03|2025-10-10~2025-10-12|2025-11-20~2025-11-22|2025-12-05~2025-12-07'
WHERE [Name] = 'Rohit Mehta';
