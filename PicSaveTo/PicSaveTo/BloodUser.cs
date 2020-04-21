using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSaveTo
{
    public class BloodUser
    {
        //public int ID { get; set; }
        public string HisID { get; set; }
        public string Name { get; set; }
        //用血者身份证
        public string userIDcard { get; set; }
        //献血者证件
        public string Devotee { get; set; }
        //用血证明
        public string Hp_settlement { get; set; }
        //出院发票
        public string Pre_settlement { get; set; }
        //用血者与献血者关系证明
        public string Voucher { get; set; }
        //录入人
        public string done_settlement { get; set; }
        //报销时间
        public DateTime Refund_date { get; set; }

    }
}
