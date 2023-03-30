namespace InstrumentHiringSystem.Models.ViewModels
{
	public class ShoppingCartViewModel
	{
		public OrderHeader OrderHeader { get; set; }

		public IEnumerable<ShoppingCart> CartList { get; set; }
	}
}
