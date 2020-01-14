using System;
using System.Collections.Generic;
using System.Text;

namespace dotNetLab.Data
{
    public interface IKeyValue 
    {
        
       
        List<String> GetAllViewNames(DbInteractiveArgs args = null);

        List<String> GetAllTableNames(DbInteractiveArgs args = null);
        List<String> GetTableColumnNames(String tableName,DbInteractiveArgs args = null);
        List<String> GetTableColumnTypes(String tableName, DbInteractiveArgs args = null);
    }
}
