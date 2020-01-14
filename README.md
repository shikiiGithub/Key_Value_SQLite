# Key_Value_SQLite 
Store Local Data Like Using Key-Value DB(有问题可以联系我：qq:1548492402)
## 桌面版

### 请使用“Key_Value_Sqlite” 这个项目
### 1.first step: add following codes（在程序的Main方法中添加如下代码，这样就可以在程序的任意一个地方访问到sqlite 并快速读写数据）。
![0.PNG](https://github.com/shikiiGithub/Key_Value_SQLite/blob/master/0.PNG)
### 2.the testing winform layout (测试用界面)：
![1.PNG](https://github.com/shikiiGithub/Key_Value_SQLite/blob/master/1.PNG)
### 3.usage(简单读写)：
![2.PNG](https://github.com/shikiiGithub/Key_Value_SQLite/blob/master/2.PNG)
### 4.if you want to edit or lookup in vs at real-time ,you need a vsix called "SQLite/SQL Server Compact Toolbox". you can download it below 
[SQLite/SQL Server Compact Toolbox](https://marketplace.visualstudio.com/items?itemName=ErikEJ.SQLServerCompactSQLiteToolbox)(如果你想在vs中编辑或者查看sqlite数据库可以使用这个VS插件(我自己的sqlite插件将在不久推送到VS Market))

## 移动跨平台：
### 请使用“Key_Value_Sqlite.NETStandard” 这个项目
0.先在共享项目中App.xaml.cs 中添加一个public static Assembly assembly_sqliteProvider =null; 
1.先到安卓项目中添加对System.Data.SQLite的引用
2.在MainActivity.cs 构造方法中使用 App.assembly_sqliteProvider= typeof(Mono.Data.Sqlite.SqliteCommand).Assembly;
3.App.xaml.cs的构造方法中（在MainPage 初始化之前）：
```c#
            SQLiteDBEngine.assembly_Sqlite_Connection = assembly_sqliteProvider;
			String basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			SQLiteDBEngine engine = new SQLiteDBEngine(Path.Combine(basePath,"shikii")) ; //数据库文件名可以不写后缀
			engine.Connect() ;
```
            