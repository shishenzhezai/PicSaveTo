using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PicSaveTo
{
    public class DbFunction
    {
        SqlConnection conn;
        SqlCommand cmd;
        string sr = "server=192.168.0.102;database=yblr;uid=sa;pwd=123";
        public DbFunction()
        {
            conn = new SqlConnection(sr);//通过构造函数连接数据库
            cmd = conn.CreateCommand();//创建一个与SqlConnection(表示一个连接数据库的链接)和SqlCommand(表示要对数据库进行操作的SQL语句)相关联的的对象
        }

        ///// <summary>
        ///// 查询
        ///// </summary>
        ///// <returns></returns>
        //public List<TelObject> select()
        //{
        //    List<TelObject> list = null;
        //    cmd.CommandText = "select * from tel";//存储SQL语句
        //    conn.Open();//打开数据库连接
        //    SqlDataReader dr = cmd.ExecuteReader();//SqlDataReader 提供一种从数据库读取行的只进流方式 ExecuteReader:将 CommandText 发送到 Connection 并生成一个 SqlDataReader

        //    if (dr.HasRows)//判断dr中有没有数据
        //    {
        //        list = new List<TelObject>();
        //        while (dr.Read())//dr中读取到的行 返回true 读取的行会先存在dr中 
        //        {
        //            TelObject t = new TelObject();//这句如果放在while外面，就会只造了一个TelObject对象 只会带着一个对象的值存入list集合中
        //            t.Code = int.Parse(dr["code"].ToString());
        //            t.Name = dr["name"].ToString();
        //            t.Sex = dr["sex"].ToString();
        //            t.Phone = dr["iph"].ToString();
        //            list.Add(t);
        //        }
        //    }
        //    cmd.Dispose();//销毁cmd
        //    conn.Close();//关闭数据库连接
        //    return list;
        //}

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="code"></param>
        public void delete(int code)
        {
            //cmd.CommandText = "delete from tel where code=@code";//使用@占位符 可以避免注入攻击
            //cmd.Parameters.Clear();
            //cmd.Parameters.Add("@code", code);//给占位符赋值
            //conn.Open();
            //cmd.ExecuteNonQuery();//执行SQL语句，并返回受影响的行
            //cmd.Dispose();
            //conn.Close();
        }

        /// <summary>
        /// 单独查询
        /// </summary>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public List<BloodUser> SelectUserByName(string name)
        {
            List<BloodUser> list = null;
            cmd.CommandText = "select id 编号,useblood_name 用血者,refund_date 报销时间  from [dbo].[blood_refund_record] where useblood_name=@name";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@name", name);
            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                list = new List<BloodUser>();
                while (dr.Read())
                {
                    BloodUser b = new BloodUser();
                    b.HisID = dr["编号"].ToString();
                    b.Name = dr["用血者"].ToString();
                    b.Refund_date = DateTime.Parse(dr["报销时间"].ToString());
                    list.Add(b);
                }
            }
            cmd.Dispose();
            conn.Close();
            return list;
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="user"></param>
        public void InsertVoucherFile(BloodUser user)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into voucherFile values(");
            sb.Append("@userIDcard,@Devotee,@HisID,@regdate,@Hp_settlement,@Pre_settlement,@done_settlement,@Voucher)");
            cmd.CommandText = sb.ToString();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@userIDcard", string.IsNullOrEmpty(user.userIDcard) ? "" : user.userIDcard);
            cmd.Parameters.AddWithValue("@Devotee", string.IsNullOrEmpty(user.Devotee) ? "" : user.Devotee);
            cmd.Parameters.AddWithValue("@HisID", string.IsNullOrEmpty(user.HisID) ? "" : user.HisID);
            cmd.Parameters.AddWithValue("@regdate", DateTime.Now);
            cmd.Parameters.AddWithValue("@Hp_settlement", string.IsNullOrEmpty(user.Hp_settlement) ? "" : user.Hp_settlement);
            cmd.Parameters.AddWithValue("@Pre_settlement", string.IsNullOrEmpty(user.Pre_settlement) ? "" : user.Pre_settlement);
            cmd.Parameters.AddWithValue("@done_settlement", string.IsNullOrEmpty(user.done_settlement) ? "" : user.done_settlement);
            cmd.Parameters.AddWithValue("@Voucher", string.IsNullOrEmpty(user.Voucher) ? "" : user.Voucher);
            conn.Open();
            cmd.ExecuteNonQuery();//执行SQL语句，并返回受影响的行
            cmd.Dispose();
            conn.Close();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="user"></param>
        public void UpdataVoucherFile(BloodUser user, string field)
        {
            StringBuilder sb = new StringBuilder("update voucherFile set ");
            sb.Append(field + "=@field ,");
            sb.Append("done_settlement=@done_settlement ");
            sb.Append(" where HisID=@hisID");

            cmd.CommandText = sb.ToString();
            cmd.Parameters.Clear();

            Type t = user.GetType();
            PropertyInfo piName = t.GetProperty(field);
            //给Name属性赋值
            string text = piName.GetValue(user).ToString();

            cmd.Parameters.AddWithValue("@field", text);
            cmd.Parameters.AddWithValue("@done_settlement", user.done_settlement);
            cmd.Parameters.AddWithValue("@hisID", user.HisID);

            conn.Open();
            cmd.ExecuteNonQuery();//执行SQL语句，并返回受影响的行
            cmd.Dispose();
            conn.Close();
        }

        public bool ExistVoucherFile(string hisID)
        {
            bool flag = false;
            cmd.CommandText = "select * from voucherFile where HisID=@hisID";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@hisID", hisID);

            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                flag = true;
            }
            cmd.Dispose();
            conn.Close();

            return flag;
        }

        ///// <summary>
        ///// 模糊查询
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="sex"></param>
        ///// <returns></returns>
        //public List<TelObject> select(string name, string sex)
        //{
        //    List<TelObject> list = null;
        //    cmd.CommandText = "select * from tel where name like '%" + name + "%' and sex like '%" + sex + "%'";
        //    //这个地方不能用占位符，不解，暂时用拼接字符串吧         
        //    conn.Open();
        //    SqlDataReader dr = cmd.ExecuteReader();//SqlDataReader 提供一种从数据库读取行的只进流方式 ExecuteReader:将 CommandText 发送到 Connection 并生成一个 SqlDataReader

        //    if (dr.HasRows)//判断dr中有没有数据
        //    {
        //        list = new List<TelObject>();
        //        while (dr.Read())//dr中读取到的行 返回true 读取的行会先存在dr中 
        //        {
        //            TelObject t = new TelObject();//这句如果放在while外面，就会只造了一个TelObject对象 只会带着一个对象的值存入list集合中
        //            t.Code = int.Parse(dr["code"].ToString());
        //            t.Name = dr["name"].ToString();
        //            t.Sex = dr["sex"].ToString();
        //            t.Phone = dr["iph"].ToString();
        //            list.Add(t);
        //        }
        //    }
        //    cmd.Dispose();//销毁cmd
        //    conn.Close();//关闭数据库连接
        //    return list;
        //}
    }
}

