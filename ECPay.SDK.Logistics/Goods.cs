namespace ECPay.SDK.Logistics
{
    /// <summary>商品</summary>
    public class Goods
    {
        /// <summary>商品名稱</summary>
        public string GoodsName { get; set; }

        /// <summary>商品金額：僅可使用數字。</summary>
        public decimal GoodsAmount { get; set; }

        /// <summary>數量</summary>
        public int Quantity { get; set; }

        /// <summary>成本</summary>
        public decimal Cost { get; set; }
    }
}