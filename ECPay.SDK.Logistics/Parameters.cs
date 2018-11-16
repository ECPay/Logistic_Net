using System;
using System.Collections.Generic;

namespace ECPay.SDK.Logistics
{
    public class Parameters
    {
        #region public properties

        /// <summary>廠商交易編號(僅可使用大小寫英文與數字)</summary>
        public string MerchantTradeNo { get; set; }

        /// <summary>物流子類型 <see cref="LogisticsSubTypes"/></summary>
        public LogisticsSubTypes? LogisticsSubType { get; set; }

        /// <summary>是否代收貨款 <see cref="IsCollections"/></summary>
        public IsCollections? IsCollection { get; set; }

        /// <summary>伺服器端回復網址</summary>
        /// <remarks>取得超商店鋪代號等資訊後，會回傳到此網址</remarks>
        public string ServerReplyURL { get; set; }

        /// <summary>使用設備 <see cref="Devices"/></summary>
        /// <remarks>根據Device類型，來顯示相對應知電子地圖</remarks>
        public Devices? Device { get; set; }

        /// <summary>額外資訊(供廠商傳遞保留的資訊，在回傳參數中，會原值回傳)</summary>
        public string ExtraData { get; set; }

        /// <summary>Html按鈕顯示名稱</summary>
        public string ButtonText { get; set; }

        /// <summary>廠商交易時間</summary>
        public string MerchantTradeDate { get; set; }

        /// <summary>代收金額</summary>
        public decimal? CollectionAmount { get; set; }

        /// <summary>商品名稱</summary>
        public string GoodsName { get; set; }

        /// <summary>商品金額</summary>
        public decimal GoodsAmount { get; set; }

        /// <summary>寄件人姓名</summary>
        public string SenderName { get; set; }

        /// <summary>寄件人電話：僅可使用0~9為字串內容，長度限制為7~20</summary>
        public string SenderPhone { get; set; }

        /// <summary>寄件人手機：僅可使用0~9為字串內容，長度限制為7~20</summary>
        public string SenderCellPhone { get; set; }

        /// <summary>收件人姓名</summary>
        public string ReceiverName { get; set; }

        /// <summary>收件人電話：僅可使用0~9為字串內容，長度限制為7~20</summary>
        public string ReceiverPhone { get; set; }

        /// <summary>收件人手機：僅可使用0~9為字串內容，長度限制為7~20</summary>
        public string ReceiverCellPhone { get; set; }

        /// <summary>收件人電子郵件</summary>
        public string ReceiverEmail { get; set; }

        /// <summary>交易敘述</summary>
        public string TradeDesc { get; set; }

        /// <summary>客戶端回覆網址</summary>
        public string ClientReplyURL { get; set; }

        /// <summary>伺服器端物流回傳網址</summary>
        public string LogisticsC2CReplyURL { get; set; }

        /// <summary>備註</summary>
        public string Remark { get; set; }

        /// <summary>專案合作平台商代號</summary>
        /// <remarks>
        /// 由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。
        /// 若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。
        /// </remarks>
        public string PlatformID { get; set; }

        /// <summary>寄件人郵遞區號</summary>
        public string SenderZipCode { get; set; }

        /// <summary>寄件人地址</summary>
        public string SenderAddress { get; set; }

        /// <summary>收件人郵遞區號</summary>
        public string ReceiverZipCode { get; set; }

        /// <summary>收件人地址</summary>
        public string ReceiverAddress { get; set; }

        /// <summary>溫層 <see cref="Temperatures"/></summary>
        /// <remarks>0001:常溫(預設值)</remarks>
        public Temperatures? Temperature { get; set; }

        /// <summary>距離 <see cref="Distances"/></summary>
        /// <remarks>00:同縣市(預設值)</remarks>
        public Distances? Distance { get; set; }

        /// <summary>規格 <see cref="Specifications"/></summary>
        /// <remarks>60cm (預設值)</remarks>
        public Specifications? Specification { get; set; }

        /// <summary>預定取件時段 <see cref="ScheduledPickupTimes"/></summary>
        /// <remarks>當子物流選擇宅配通時，該參數可不填</remarks>
        public ScheduledPickupTimes? ScheduledPickupTime { get; set; }

        /// <summary>預定送達時段 <see cref="ScheduledDeliveryTimes"/></summary>s
        public ScheduledDeliveryTimes? ScheduledDeliveryTime { get; set; }

        /// <summary>指定送達日</summary>
        /// <remarks>當子物流選擇宅配通時，此參數才有作用日期指定限制D+3(D:該訂單建立時間)</remarks>
        public DateTime? ScheduledDeliveryDate { get; set; }

        /// <summary>包裹件數</summary>
        /// <remarks>當子物流選擇宅配通時，此參數才有作用，作用於同訂單編號，包裹件數。</remarks>
        public int? PackageCount { get; set; }

        /// <summary>收件人門市代號</summary>
        public string ReceiverStoreID { get; set; }

        /// <summary>退貨門市代號</summary>
        /// <remarks>當為店到店的退貨時，要退回的門市代號</remarks>
        public string ReturnStoreID { get; set; }

        /// <summary>綠界科技的物流交易編號</summary>
        /// <remarks>僅可使用0~9為字串內容。請保存綠界科技的交易編號與AllPayLogisticsID的關連。</remarks>
        public string AllPayLogisticsID { get; set; }

        /// <summary>物流類型 <see cref="LogisticsTypes"/></summary>
        public LogisticsTypes? LogisticsType { get; set; }

        /// <summary>商品資料集合</summary>
        public List<Goods> Items { get; set; }

        /// <summary>物流訂單出貨日期</summary>
        public DateTime? ShipmentDate { get; set; }

        /// <summary>寄貨編號</summary>
        /// <remarks>僅可使用大小寫英文與數字。</remarks>
        public string CVSPaymentNo { get; set; }

        /// <summary>驗證碼</summary>
        /// <remarks>僅可使用0~9為字串內容。</remarks>
        public string CVSValidationNo { get; set; }

        /// <summary>門市類型 <see cref="StoreTypes"/></summary>
        /// <remarks>
        /// 1.StoreType.RECIVE_STORE(收件門市)
        /// 2.StoreType.RETURN_STORE(退貨門市)
        /// </remarks>
        public StoreTypes? StoreType { get; set; }

        #endregion public properties

        #region Response Properties

        /// <summary>托運單號</summary>
        public string BookingNote { get; set; }

        /// <summary>目前物流狀態</summary>
        public string RtnCode { get; set; }

        /// <summary>物流狀態說明</summary>
        public string RtnMsg { get; set; }

        /// <summary>物流狀態更新時間</summary>
        public DateTime? UpdateStatusDate { get; set; }

        /// <summary>User 所選之超商 店舖編號</summary>
        public string CVSStoreID { get; set; }

        /// <summary>User 所選之超商 店舖名稱</summary>
        public string CVSStoreName { get; set; }

        /// <summary>User 所選之超商 店舖地址</summary>
        public string CVSAddress { get; set; }

        /// <summary>User 所選之超商 店舖電話</summary>
        public string CVSTelephone { get; set; }

        /// <summary>退貨編號</summary>
        /// <remarks>商品欲退貨時，至統一超商 ibon 輸入之 退貨編號</remarks>
        public string RtnOrderNo { get; set; }

        /// <summary>物流費用</summary>
        public decimal HandlingCharge { get; set; }

        /// <summary>訂單成立時間</summary>
        public DateTime? TradeDate { get; set; }

        /// <summary>配送編號</summary>
        /// <remarks>物流類型為非宅配時會有回傳值</remarks>
        public string ShipmentNo { get; set; }

        /// <summary>物流狀態</summary>
        public string LogisticsStatus { get; set; }

        #endregion Response Properties

        public Parameters()
        {
            Items = new List<Goods>();
            LogisticsSubType = LogisticsSubTypes.None;
        }
    }

    public class ParameterValue
    {
        public string Value { get; set; }
        public bool Required { get; set; }
        private Action condition;

        public ParameterValue(string value, bool required)
        {
            Value = value;
            Required = required;
        }

        public ParameterValue(string value, bool required, Action condition)
        {
            Value = value;
            Required = required;
            this.condition = condition;
        }
    }
}