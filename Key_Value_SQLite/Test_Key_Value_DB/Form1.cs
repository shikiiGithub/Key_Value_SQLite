using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using dotNetLab.Data;
namespace Test_Key_Value_DB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
           
            InitializeComponent();
        }

        private void Btn_Read_Click(object sender, EventArgs e)
        {
            // 可以不填写表名，这种情况下保存在默认表里
            //需要注意的是当指定的键不存在时会自动创建这个键并会给键赋值为“0”;
            //如不需要这个自动添加键的功能请使用
            //Program.CompactDB.FetchValue(txb_KeyName.Text.Trim(), false); 或者 Program.CompactDB.FetchValue(txb_TableName.Text.Trim(), txb_KeyName.Text.Trim(), false);
            //may not input a table name, if you dont provide a table name the engine will use the default table name instead .
            //the key's value will be assigned as "0" when  the key does not exist.
            //if you dont like this auto opration above,you can code as the following:
            //Program.CompactDB.FetchValue(txb_KeyName.Text.Trim(), false); 或者 Program.CompactDB.FetchValue(txb_TableName.Text.Trim(), txb_KeyName.Text.Trim(), false);

            if (String.IsNullOrEmpty(txb_TableName.Text))
            {
                txb_ValueName.Text = Program.CompactDB.FetchValue(txb_KeyName.Text.Trim());
            }
            else
                Program.CompactDB.FetchValue(txb_TableName.Text.Trim(), txb_KeyName.Text.Trim());
        }

        private void Btn_Write_Click(object sender, EventArgs e)
        {
            // 可以不填写表名，这种情况下保存在默认表里
            //更新数据与写入均使用Write方法
            //Write Method Used as new or update key value ;
            if (String.IsNullOrEmpty(txb_TableName.Text))
            {
                Program.CompactDB.Write(  txb_KeyName.Text.Trim(), txb_ValueName.Text.Trim());
            }
            else
                Program.CompactDB.Write(txb_TableName.Text.Trim(),txb_KeyName.Text.Trim(), txb_ValueName.Text.Trim());


        }
    }
}
