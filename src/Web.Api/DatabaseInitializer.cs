namespace Web.Api
{
	public class DatabaseInitializer
	{
	}

	public class StockPrice
	{
		public int Id { get; set; }
		public string Ticker { get; set; }
		public decimal Price { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
