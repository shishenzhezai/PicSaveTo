using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Reflection;

namespace PicSaveTo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DbFunction dbf;
        string oldPath = "";
        string newPath = "";
        Dictionary<string, string> dic = new Dictionary<string, string>();

        private void Form1_Load(object sender, EventArgs e)
        {
            dbf = new DbFunction();
            dic.Add("身份证", "userIDcard");
            dic.Add("用血证明", "Hp_settlement");
            dic.Add("发票", "Pre_settlement");
            dic.Add("献血证", "Devotee");
            dic.Add("关系证明", "Voucher");
            this.textBox1.Text = @"E:\FDFF\oldFile";
            this.textBox2.Text = @"E:\FDFF\newFile";
            this.entrantCount.Text = "0";
            this.ProcessedCount.Text = "0";
        }

        /// <summary>
        /// 选择读取文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dilog = new FolderBrowserDialog();
            dilog.Description = "请选择文件夹";
            if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
            {
                textBox1.Text = dilog.SelectedPath;
            }
        }

        private void GetFiles()
        {
            try
            {
                string baseDirectory = this.textBox1.Text;
                if (!string.IsNullOrEmpty(baseDirectory))
                {
                    var directory = Directory.GetDirectories(baseDirectory);
                    foreach (var dir in directory)
                    {
                        string entrant = dir.Substring(dir.LastIndexOf('\\') + 1);//录入人
                        this.textBox3.Text = entrant;
                        var files = Directory.GetFiles(dir);
                        this.entrantCount.Text = files.Length.ToString();
                        int processedCount = 0;
                        this.ProcessedCount.Text = processedCount.ToString();
                        foreach (var file in files)
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            string filename = fileInfo.Name;
                            string fileNameNoSuffix = filename.Remove(filename.IndexOf('.'));
                            var arrayofname = fileNameNoSuffix.Split('_');
                            var bmp = ReadImageFile(fileInfo.FullName);
                            if (bmp != null)
                            {
                                List<BloodUser> list = dbf.SelectUserByName(arrayofname[0]);
                                if (list != null)
                                {
                                    BloodUser user = null;
                                    //判断图片的报销日期
                                    if (!string.IsNullOrWhiteSpace(arrayofname[2]))
                                    {
                                        DateTime refund_date = StringToDate(arrayofname[2]);
                                        user = list.FirstOrDefault(x => x.Refund_date == refund_date);
                                        if (user == null)
                                        {
                                            user = list.FirstOrDefault();
                                        }
                                    }
                                    else
                                    {
                                        user = list[0];
                                    }
                                    if (user != null)
                                    {
                                        string his_ID = user.HisID;
                                        string oldName = filename;
                                        string prefix = NameConvert(arrayofname[1]);
                                        string newName = prefix + oldName;
                                        #region 保存文件
                                        SavePic(bmp, prefix + oldName, his_ID, entrant);
                                        #endregion
                                        #region 保存数据库                                    
                                        Type t = user.GetType();
                                        PropertyInfo piName = t.GetProperty(prefix);
                                        //给对应字段设置图片地址
                                        piName.SetValue(user, "\\" + prefix + oldName, null);

                                        user.done_settlement = entrant;
                                        //判断voucherFile是否存在记录
                                        if (dbf.ExistVoucherFile(user.HisID))
                                        {
                                            dbf.UpdataVoucherFile(user, prefix);
                                        }
                                        else
                                        {
                                            dbf.InsertVoucherFile(user);
                                        }
                                        #endregion
                                        //删除原文件
                                        fileInfo.Delete();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("未查询到图片姓名：" + arrayofname[0]);
                                }
                            }
                            else
                            {
                                MessageBox.Show("图片读取失败，请检查路径");
                            }

                            #region 更新已完成数量
                            processedCount++;
                            this.ProcessedCount.Text = processedCount.ToString();
                            #endregion
                        }
                    }
                }
                MessageBox.Show("保存完成");
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// 选择保存文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dilog = new FolderBrowserDialog();

            dilog.Description = "请选择文件夹";
            if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
            {
                textBox2.Text = dilog.SelectedPath;
            }
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="bmp">图片</param>
        /// <param name="fileName">文件名</param>
        /// <param name="hisID">hisID</param>
        /// <param name="entrant">录入人</param>
        public void SavePic(Bitmap bmp, string fileName, string hisID, string entrant)
        {
            string saveToPath = this.textBox2.Text;
            //判断文件夹是否存在
            saveToPath = saveToPath + "\\" + entrant + "\\" + hisID;
            if (!string.IsNullOrEmpty(saveToPath))
            {
                if (!Directory.Exists(saveToPath))
                {
                    Directory.CreateDirectory(saveToPath);
                }
                bmp.Save(saveToPath + "\\" + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            oldPath = this.textBox1.Text;
            newPath = this.textBox2.Text;
            if (string.IsNullOrEmpty(oldPath))
            {
                MessageBox.Show("请选择图片目录");
                return;
            }
            if (string.IsNullOrEmpty(newPath))
            {
                MessageBox.Show("请选择保存目录");
                return;
            }
            if (!string.IsNullOrEmpty(oldPath) && !string.IsNullOrEmpty(newPath))
            {
                GetFiles();
            }
        }

        private string NameConvert(string Name)
        {
            //List<string> list1 = new List<string> { "身份证", "用血证明", "发票", "献血证", "关系证明", "清单" };
            //List<str>

            var result = dic.FirstOrDefault(x => Regex.IsMatch(Name, x.Key));
            if (!string.IsNullOrWhiteSpace(result.Value))
            {
                return result.Value;
            }
            return null;
        }

        /// <summary>
        /// 读取图片文件
        /// </summary>
        /// <param name="path">图片文件路径</param>
        /// <returns>图片文件</returns>
        private Bitmap ReadImageFile(String path)
        {
            Bitmap bitmap = null;
            try
            {
                FileStream fileStream = File.OpenRead(path);
                Int32 filelength = 0;
                filelength = (int)fileStream.Length;
                Byte[] image = new Byte[filelength];
                fileStream.Read(image, 0, filelength);
                System.Drawing.Image result = System.Drawing.Image.FromStream(fileStream);
                fileStream.Close();
                bitmap = new Bitmap(result);
            }
            catch (Exception ex)
            {
                //  异常输出
            }
            return bitmap;
        }

        /// <summary>
        /// 将时间字符串转换为时间
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private DateTime StringToDate(string str)
        {
            int year = Convert.ToInt32(str.Substring(0, 4));
            int mon = Convert.ToInt32(str.Substring(4, 2));
            int day = Convert.ToInt32(str.Substring(6, 2));
            DateTime date = new DateTime(year, mon, day);
            return date;
        }
    }
}
