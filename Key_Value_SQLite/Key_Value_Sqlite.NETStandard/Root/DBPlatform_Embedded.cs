using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace dotNetLab.Data
{
  public partial  class DBPlatform
    {
        public string DefaultTable => "App_Extension_Data_Table";
        public void Write(string strTableName, string strName, String strValue,DbInteractiveArgs args = null)
        {
            if (!bConnected)
            {
                ErrorHandler?.Invoke(this, new Exception(BADCONNECTION));
                return;
            }
            if (IsExistRecord(strName, strTableName,args))
                this.Update(strTableName, string.Format("Val='{0}' Where Name='{1}' ", strValue, strName),args);
            else
                this.NewRecord(strTableName, string.Format("'{0}','{1}'", strName, strValue),args);
           
        }
        public void Write(string strName, string strValue,DbInteractiveArgs args = null)
        {
            Write(DefaultTable, strName, strValue,args);
        }
        public string FetchValue( String strTableName,String strLabelName, bool NewWhenNoFound = true,
            String strDefaultValue = "0",DbInteractiveArgs args = null)
        {

            cc:;
            if (!bConnected)
            {
                ErrorHandler?.Invoke(this, new Exception(BADCONNECTION));
                return null;
            }

            String temp = this.UniqueResult(
                string.Format("select Val from {0} where Name='{1}'; "
                , strTableName, strLabelName),args);
            if (temp == null && NewWhenNoFound)
            {
                Write(strTableName, strLabelName, strDefaultValue,args);
                goto cc;
            }
            else
            {
                return temp;
            }
        }
        public String FetchValue(String strLabelName, bool NewWhenNoFound = true, String strDefaultValue = "0",DbInteractiveArgs args = null)
        {
            return FetchValue(DefaultTable, strLabelName,  NewWhenNoFound, strDefaultValue, args);
        }
        public int FetchIntValue(string strLabelName, bool NewWhenNoFound = true, String strDefaultValue = "0", DbInteractiveArgs args = null)
        {
            string temp = FetchValue(DefaultTable, strLabelName,  NewWhenNoFound,  strDefaultValue ,args);
            try
            {
                return int.Parse(temp);
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, e);
                return nError;
            }


        }
        public int FetchIntValue(string strTableName, string strLabelName,  bool NewWhenNoFound = true, String strDefaultValue = "0", DbInteractiveArgs args = null)
        {
            string temp = FetchValue( strTableName,strLabelName, NewWhenNoFound, strDefaultValue, args);
            try
            {
                return int.Parse(temp);
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, e);
                return nError;
            }


        }
        public float FetchFloatValue(string strLabelName, bool NewWhenNoFound = true, String strDefaultValue = "0", DbInteractiveArgs args = null)
        {
            string temp = FetchValue(DefaultTable, strLabelName, NewWhenNoFound, strDefaultValue, args);
            try
            {
                return float.Parse(temp);
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, e);
                return nError;
            }
        }
        public double FetchDoubleValue(string strLabelName, bool NewWhenNoFound = true, String strDefaultValue = "0", DbInteractiveArgs args = null)
        {
            string temp = FetchValue(DefaultTable, strLabelName, NewWhenNoFound, strDefaultValue, args);
            try
            {
                return double.Parse(temp);
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, e);
                return nError;
            }

        }
        public DateTime FetchDateTimeValue(string strLabelName, bool NewWhenNoFound = true, String strDefaultValue = "0", DbInteractiveArgs args = null)
        {
            string temp = FetchValue(DefaultTable, strLabelName,  NewWhenNoFound, strDefaultValue, args);
            try
            {
                return DateTime.Parse(temp);
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, e);
                return DateTime.MinValue;
            }
        }
        public bool FetchBoolValue(string strLabelName, bool NewWhenNoFound = true, String strDefaultValue = "0", DbInteractiveArgs args = null)
        {
            string temp = FetchValue(DefaultTable, strLabelName, NewWhenNoFound, strDefaultValue, args);
            try
            {
                return Boolean.Parse(temp);
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, e);
                return false;
            }
        }
        public void WriteArray<T>(String strTableName, String strName, T[] tArr,  DbInteractiveArgs args = null)
        {

            for (int i = 0; i < tArr.Length; i++)
            {
                this.AppendItem(strTableName, strName, tArr[i].ToString(),args);
            }

        }
        public void WriteArray<T>(String strName, T[] tArr, DbInteractiveArgs args = null)
        {

            WriteArray<T>(DefaultTable, strName, tArr,args);

        }
        public void AppendItem(string strLabelName, String obj, DbInteractiveArgs args =null)
        {
            AppendItem(DefaultTable, strLabelName, obj,args);
        }

        //添加一个数组元素，这个数据元素可以与原有的数组元素相同。
        //如果这个键值对不存在，创建这个键值对并返回。
        //如果这个键值对存在则将之前的值与新增的值组成一个数组再写入到这个键所对应的值 
        public void AppendItem(string strTableName, string strLabelName, String obj,DbInteractiveArgs args = null)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                String[] strArr_Write = null;
                //先尝试取出字符串数组
                String[] strArr = FetchArray(strTableName, strLabelName,args);
                if (obj == null)
                    return;
                //如果不是数组
                if (strArr == null)
                {
                    
                    //取出这个值（键值对）
                    String str = FetchValue(strTableName, strLabelName,false,"0",args);
                   //如果这个键值对不存在
                    if (str == null || str.Equals(""))
                    {
                        //创建这个键值对
                        Write(strTableName, strLabelName, obj.ToString(),args);
                        return;
                    }
                    //如果这个键值对存在则将之前的值与新增的值组成一个数组。
                    else
                    {
                        
                        strArr_Write = new String[] { str, obj };
                    }

                }
                else
                {
                    strArr_Write = new String[strArr.Length + 1];
                    for (int i = 0; i < strArr.Length; i++)
                    {
                        strArr_Write[i] = strArr[i];
                    }
                    strArr_Write[strArr_Write.Length - 1] = obj;
                }

                //将新增的数据写入
                for (int i = 0; i < strArr_Write.Length; i++)
                {
                    if (i == 0)
                    {
                        sb.Append(strArr_Write[i]);
                    }
                    else
                        sb.Append(String.Format("{0}{1}", SPLITMARK, strArr_Write[i]));
                }
                this.Write(strTableName, strLabelName, sb.ToString(),args);
                sb.Remove(0, sb.Length);
                sb = null;
            }
            catch (Exception ex)
            {

                ErrorHandler?.Invoke(this, ex);

            }
           

        }
        //添加一个数组元素，这个数据元素不能与原有的数组元素相同。
        //如果这个键值对不存在，创建这个键值对并返回。
        //如果这个键值对存在则将之前的值与新增的值组成一个数组再写入到这个键所对应的值 
        public void AppendUniqueItem(String strTableName, String strLabelName, String obj,DbInteractiveArgs args = null)
        {
            String[] strArr_Write = null;
            String[] strArr = FetchArray(strTableName, strLabelName,args);
            if (obj == null)
                return;
            if (strArr == null)
            {
               
                String str = FetchValue(strTableName, strLabelName,false,"-69",args);
                if (str == null || str.Equals(""))
                {
                    Write(strTableName, strLabelName, obj.ToString(),args);
                    return;
                }
                else
                {
                    strArr_Write = new String[] { str, obj };
                }
            }
            else
            {
                strArr_Write = new String[strArr.Length + 1];
                for (int i = 0; i < strArr.Length; i++)
                {
                    strArr_Write[i] = strArr[i];
                }
                strArr_Write[strArr_Write.Length - 1] = obj;
            }
            StringBuilder sb = new StringBuilder();
            List<String> lst_Arr = new List<String>();
           
            if (strArr == null)
            {
                for (int i = 0; i < strArr_Write.Length; i++)
                {
                    if (lst_Arr.Contains(strArr_Write[i]))
                        continue;
                    lst_Arr.Add(strArr_Write[i]);
                }
            }
            else
            {
                for (int j = 0; j < strArr_Write.Length; j++)
                {
                    if (!lst_Arr.Contains(strArr_Write[j]))
                        lst_Arr.Add(strArr_Write[j]);
                }
            }


            for (int i = 0; i < lst_Arr.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(lst_Arr[i]);
                }
                else
                    sb.Append(String.Format("{0}{1}", SPLITMARK, lst_Arr[i]));
            }
            this.Write(strTableName, strLabelName, sb.ToString(),args);
            sb.Remove(0, sb.Length);
            sb = null;
        }
        public void AppendUniqueItem( String strLabelName, String obj, DbInteractiveArgs args = null)
        {
            AppendUniqueItem(DefaultTable, strLabelName, obj, args);
        }
        public void AppendDynamicItem(String strTableName, String strLabelName, dynamic obj,bool ElementCanRepeat, DbInteractiveArgs args = null)
        {
            Type type =  obj.GetType();
            PropertyInfo[] pifs = type.GetProperties();

            foreach (PropertyInfo item in pifs)
            {
                Object o = item.GetValue(obj, null);
                if(ElementCanRepeat)
                  AppendItem( strTableName,strLabelName,o.ToString(),args);
                else
                    AppendUniqueItem(strTableName, strLabelName, o.ToString(), args);
            }
        }
        public void AppendDynamicItem(String strLabelName, dynamic obj, bool ElementCanRepeat, DbInteractiveArgs args = null)
        {
            Type type = obj.GetType();
            PropertyInfo[] pifs = type.GetProperties();


            foreach (var item in pifs)
            {
                Object o = item.GetValue(obj, null);
                if (ElementCanRepeat)
                    AppendItem(DefaultTable, strLabelName, o.ToString(), args);
                else
                    AppendUniqueItem(DefaultTable, strLabelName, o.ToString(), args);
            }
        }
        public String[] FetchArray(string strLabelName,DbInteractiveArgs args = null)
        {

            /* String str = FetchValue(strLabelName);
             if (str.Contains(SPLITMARK.ToString()))
                 return str.Split(new char[] { SPLITMARK });
             else
                 return null;*/
            return FetchArray("App_Extension_Data_Table", strLabelName,args);
        }
        public String[] FetchArray(String strTableName, string strLabelName,DbInteractiveArgs args=null)
        {

            
            String str = FetchValue(strLabelName, strTableName,false,"-89",args);

            if (str == null)
                return null;
            else
            {
                if (str.Contains(SPLITMARK.ToString()))
                    try
                    {
                        String[] str_ = str.Split('^');
                        return str_;
                    }
                    catch (Exception e)
                    {

                        return null;
                    }
                else
                    return new String[] { str };
            }
        }
        public void FetchFloatArray(string strLabelName, out float[] fArr,DbInteractiveArgs args = null)
        {
            String[] pArr = FetchArray(strLabelName,args);
            fArr = new float[pArr.Length];
            for (int i = 0; i < fArr.Length; i++)
            {
                fArr[i] = Convert.ToSingle(pArr[i]);
            }

        }
        public void FetchDoubleArray(string strLabelName, out double[] lfArr,DbInteractiveArgs args = null)
        {
            String[] pArr = FetchArray(strLabelName,args);
            lfArr = new double[pArr.Length];
            for (int i = 0; i < lfArr.Length; i++)
            {
                lfArr[i] = Convert.ToDouble(pArr[i]);
            }
        }
        public void FetchIntArray(String strLabelName, out int[] arr,DbInteractiveArgs args = null)
        {
            String[] pArr = FetchArray(strLabelName,args);
            arr = new int[pArr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = Convert.ToInt32(pArr[i]);
            }
        }
        protected bool IsExistRecord(string strName, string Tablename,DbInteractiveArgs args = null)
        {
         
            string temp = this.UniqueResult(
                string.Format("select count(*) from {0} where Name='{1}'"
                 , Tablename, strName
                 ),args);
            try
            {
                if (int.Parse(temp) > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {

                return false;
            }

        }
       
    }
}
