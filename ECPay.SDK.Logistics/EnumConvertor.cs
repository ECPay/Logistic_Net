namespace ECPay.SDK.Logistics
{
    public static class EnumConvertor
    {
        public static string ToString(Temperatures Temperature)
        {
            string result;
            switch (Temperature)
            {
                case Temperatures.ROOM:
                    result = "0001";
                    break;

                case Temperatures.REFRIGERATION:
                    result = "0002";
                    break;

                case Temperatures.FREEZE:
                    result = "0003";
                    break;

                default:
                    result = "0001";
                    break;
            }
            return result;
        }

        public static string ToString(Distances Distance)
        {
            string result;
            switch (Distance)
            {
                case Distances.SAME:
                    result = "00";
                    break;

                case Distances.OTHER:
                    result = "01";
                    break;

                case Distances.ISLAND:
                    result = "02";
                    break;

                default:
                    result = "00";
                    break;
            }
            return result;
        }

        public static string ToString(Specifications Specification)
        {
            string result;
            switch (Specification)
            {
                case Specifications.CM_60:
                    result = "0001";
                    break;

                case Specifications.CM_90:
                    result = "0002";
                    break;

                case Specifications.CM_120:
                    result = "0003";
                    break;

                case Specifications.CM_150:
                    result = "0004";
                    break;

                default:
                    result = "0001";
                    break;
            }
            return result;
        }

        public static string ToString(ScheduledPickupTimes ScheduledPickupTime)
        {
            string result;
            switch (ScheduledPickupTime)
            {
                case ScheduledPickupTimes.TIME_9_12:
                    result = "1";
                    break;

                case ScheduledPickupTimes.TIME_12_17:
                    result = "2";
                    break;

                case ScheduledPickupTimes.TIME_17_20:
                    result = "3";
                    break;

                case ScheduledPickupTimes.TIME_UNLIMITED:
                    result = "4";
                    break;

                default:
                    result = "";
                    break;
            }
            return result;
        }

        public static string ToString(ScheduledDeliveryTimes ScheduledDeliveryTime)
        {
            string result;
            switch (ScheduledDeliveryTime)
            {
                case ScheduledDeliveryTimes.TIME_0_13:
                    result = "1";
                    break;

                case ScheduledDeliveryTimes.TIME_14_18:
                    result = "2";
                    break;

                case ScheduledDeliveryTimes.TIME_UNLIMITED:
                    result = "4";
                    break;

                default:
                    result = "";
                    break;
            }
            return result;
        }

        public static string ToString(StoreTypes StoreType)
        {
            string result;
            switch (StoreType)
            {
                case StoreTypes.RECIVE_STORE:
                    result = "01";
                    break;

                case StoreTypes.RETURN_STORE:
                    result = "02";
                    break;

                default:
                    result = "";
                    break;
            }
            return result;
        }

        public static string ToString(Devices Device)
        {
            string result;
            switch (Device)
            {
                case Devices.PC:
                    result = "0";
                    break;

                case Devices.Mobile:
                    result = "1";
                    break;

                default:
                    result = "";
                    break;
            }
            return result;
        }
    }
}