EXEC sp_UpdateUnavailableDates 
     @Name = 'Rohit Mehta', 
     @NewDateRange = '2025-12-21~2025-12-21';
	 EXEC [dbo].[sp_UpdateUnavailableDates]
    @Name = 'Rohit Mehta',-- thsi add a new to the existing 
    @NewDateRange = '2026-01-10~2026-01-12';

	EXEC [dbo].[sp_UpdateUnavailableDates]
    @Name = 'Rohit Mehta',
    @NewDateRange = '2025-07-10~2025-07-12';
	EXEC [dbo].[sp_UpdateUnavailableDates]
    @Name = 'Rohit Mehta',
    @NewDateRange = '2025-12-05~2025-12-10';