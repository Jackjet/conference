using ConferenceCommon.LogHelper;
/*
* ----------------------------------------------------------------             
Copyright (C#) 2015 北京圣邦天麒科技有限公司 版权所有      
文件名： DBHelper
文件功能描述： 数据库管理区域
创建标识： 
修改标识： 
修改描述：     
* ----------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

/// <summary>
/// 数据库管理区域
/// </summary>
public static class DBManage
{
    #region 获取数据库连接

    public static string sqlConnection = null;


    /// <summary>
    /// 获取数据库的连接对象
    /// </summary>
    /// <returns>返回连接对象</returns>
    public static SqlConnection GetNewConnection()
    {
        //创建数据库对象
        SqlConnection conn = null;
        try
        {
            //创建数据库连接
            conn = new SqlConnection(sqlConnection);
        }
        catch (Exception ex)
        {

            LogManage.WriteLog(typeof(DBManage), ex);
        }
        return conn;
    }

    #endregion

    #region 获取数据库数据【查】【修改：将参数中object[]改为SqlParameter[]】

    /// <summary>
    /// 执行查询操作（数据库表单实体化）
    /// </summary>
    /// <typeparam name="T">指定映射的类型</typeparam>
    /// <param name="commandText">执行命令【文本】</param>
    /// <param name="commandtype">执行类型</param>
    /// <param name="errorString">异常记录</param>
    /// <param name="args">其他参数【数据库】</param>
    /// <returns>返回值</returns>
    public static List<T> ExcuteEntity<T>(string commandText, CommandType commandtype, out string errorString, params SqlParameter[] args)
    {
        //该类型实例的集合
        List<T> tList = new List<T>();
        //定义一个连接
        SqlConnection con = null;
        //异常信息
        errorString = null;
        try
        {
            //获取连接
            con = GetNewConnection();
            //创建数据库命令对象
            using (SqlCommand cmd = new SqlCommand(commandText, con))
            {
                //命令类型
                cmd.CommandType = commandtype;
                //添加数据库参数
                cmd.Parameters.AddRange(args);//存在错误！！！！！已修改
                //打开数据库连接
                con.Open();
                //创建读取器（只进只读对象）
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    //读取数据库记录【只读、只进不退】
                    while (reader.Read())
                    {
                        //执行命令
                        T objd = ExcuteReader<T>(reader);
                        //添加单条记录
                        tList.Add(objd);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //获取异常信息
            errorString = ex.Message;
            LogManage.WriteLog(typeof(DBManage), ex);
        }
        finally
        {
            if (con != null)
            {
                //释放连接
                con.Close();
            }
        }

        return tList;
    }

    /// <summary>
    /// 获取单个实体
    /// </summary>
    /// <typeparam name="T">执行类型</typeparam>
    /// <param name="reader">数据库读取器</param>
    /// <returns>返回值</returns>
    static T ExcuteReader<T>(SqlDataReader reader)
    {
        //定义默认类型对象
        T obj = default(T);
        //获取对象的类型
        Type type = typeof(T);
        //获取该类的属性集
        PropertyInfo[] propertyInfos = type.GetProperties();
        //创建实例
        obj = Activator.CreateInstance<T>();
        //获取字段数量
        int fieldsCount = reader.FieldCount;
        //遍历属性值
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            //获取当前属性的名称
            string propertyInfoName = propertyInfo.Name;
            //遍历字段
            for (int i = 0; i < fieldsCount; i++)
            {
                //读取数据库的字段名称
                string fieldName = reader.GetName(i);
                //对比是否和实体属性的名称相对应
                if (string.Compare(propertyInfoName, fieldName, true) == 0)
                {
                    //该字段的值【数据库】
                    object objec = reader.GetValue(i);
                    try
                    {
                        //为空判断
                        if (propertyInfo.PropertyType == typeof(string) && objec == DBNull.Value)
                        {
                            objec = null;
                        }
                        //给该字段设置值
                        propertyInfo.SetValue(obj, objec, null);
                    }
                    catch (Exception ex)
                    {
                          LogManage.WriteLog(typeof(DBManage), ex);
                    }
                    break;
                }
            }
        }
        return obj;
    }

    #endregion
   
    #region 批量执行命令【增删改】

    /// <summary>
    /// 批量带有事物的 全部成功则提交 有没有成功则回滚  注意这个和上边的不一样他抛出异常了  在处理errorString时要在catch中
    /// </summary>
    /// <param name="Sqls">T-Sql语句</param>
    /// <param name="errorString">异常信息</param>
    public static void Transaction(string Sql, out string errorString)
    {
        //定义一个连接
        SqlConnection conn = null;
        //异常信息
        errorString = null;
        try
        {
            //获取连接
            conn = GetNewConnection();
            //打开数据库连接
            conn.Open();
            //创建数据库命令对象
            using (SqlCommand cmd = new SqlCommand())
            {
                //设置数据库连接对象
                cmd.Connection = conn;
                //命令文本
                cmd.CommandText = Sql;
                //执行操作
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            errorString = ex.Message + "\r\n" + ex.StackTrace;
            LogManage.WriteLog(typeof(DBManage), ex);
        }
        finally
        {
            if (conn != null)
            {
                //释放连接
                conn.Close();
            }
        }
    }

    /// <summary>
    /// 批量带有事物的 全部成功则提交 有没有成功则回滚  注意这个和上边的不一样他抛出异常了  在处理errorString时要在catch中
    /// </summary>
    /// <param name="Sqls">T-Sql语句集合</param>
    /// <param name="errorString">异常信息</param>
    public static void Transaction(List<string> Sqls, out string errorString)
    {
        //定义一个连接
        SqlConnection conn = null;
        //异常信息
        errorString = null;
        try
        {
            //获取连接
            conn = GetNewConnection();
            //打开数据库连接
            conn.Open();
            //创建数据库命令对象
            using (SqlCommand cmd = new SqlCommand())
            {
                //设置数据库连接对象
                cmd.Connection = conn;
                foreach (string Sql in Sqls)
                {
                    //命令文本
                    cmd.CommandText = Sql;
                    //执行操作
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            errorString = ex.Message + "\r\n" + ex.StackTrace;
            LogManage.WriteLog(typeof(DBManage), ex);
        }
        finally
        {
            if (conn != null)
            {
                //释放连接
                conn.Close();
            }
        }
    }

    /// <summary> 
    /// 执行指定连接字符串,类型的SqlCommand.如果没有提供参数,不返回结果. 
    /// </summary> 
    /// <remarks> 
    /// 示例:  
    ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24)); 
    /// </remarks> 
    /// <param name="connectionString">一个有效的数据库连接字符串</param> 
    /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param> 
    /// <param name="commandText">存储过程名称或SQL语句</param> 
    /// <param name="commandParameters">SqlParameter参数数组</param> 
    /// <returns>返回命令影响的行数</returns> 
    public static int ExecuteNonQuery( CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        using (SqlConnection connection = GetNewConnection())
        {
            connection.Open();

            return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
        }
    }

    /// <summary> 
    /// 执行指定数据库连接对象的命令 
    /// </summary> 
    /// <remarks> 
    /// 示例:  
    ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24)); 
    /// </remarks> 
    /// <param name="connection">一个有效的数据库连接对象</param> 
    /// <param name="commandType">命令类型(存储过程,命令文本或其它.)</param> 
    /// <param name="commandText">T存储过程名称或T-SQL语句</param> 
    /// <param name="commandParameters">SqlParamter参数数组</param> 
    /// <returns>返回影响的行数</returns> 
    public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
    {
        if (connection == null) throw new ArgumentNullException("connection");

        // 创建SqlCommand命令,并进行预处理 
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

        // Finally, execute the command 
        int retval = cmd.ExecuteNonQuery();

        // 清除参数,以便再次使用. 
        cmd.Parameters.Clear();
        if (mustCloseConnection)
            connection.Close();
        return retval;
    }

    /// <summary> 
    /// 预处理用户提供的命令,数据库连接/事务/命令类型/参数 
    /// </summary> 
    /// <param name="command">要处理的SqlCommand</param> 
    /// <param name="connection">数据库连接</param> 
    /// <param name="transaction">一个有效的事务或者是null值</param> 
    /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param> 
    /// <param name="commandText">存储过程名或都T-SQL命令文本</param> 
    /// <param name="commandParameters">和命令相关联的SqlParameter参数数组,如果没有参数为'null'</param> 
    /// <param name="mustCloseConnection"><c>true</c> 如果连接是打开的,则为true,其它情况下为false.</param> 
    private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
    {
        if (command == null) throw new ArgumentNullException("command");
        if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

        // If the provided connection is not open, we will open it 
        if (connection.State != ConnectionState.Open)
        {
            mustCloseConnection = true;
            connection.Open();
        }
        else
        {
            mustCloseConnection = false;
        }

        // 给命令分配一个数据库连接. 
        command.Connection = connection;

        // 设置命令文本(存储过程名或SQL语句) 
        command.CommandText = commandText;

        // 分配事务 
        if (transaction != null)
        {
            if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            command.Transaction = transaction;
        }

        // 设置命令类型. 
        command.CommandType = commandType;

        // 分配命令参数 
        if (commandParameters != null)
        {
            AttachParameters(command, commandParameters);
        }
        return;
    }

    /// <summary> 
    /// 将SqlParameter参数数组(参数值)分配给SqlCommand命令. 
    /// 这个方法将给任何一个参数分配DBNull.Value; 
    /// 该操作将阻止默认值的使用. 
    /// </summary> 
    /// <param name="command">命令名</param> 
    /// <param name="commandParameters">SqlParameters数组</param> 
    private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
    {
        if (command == null) throw new ArgumentNullException("command");
        if (commandParameters != null)
        {
            foreach (SqlParameter p in commandParameters)
            {
                if (p != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value. 
                    if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) &&
                        (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }
                    command.Parameters.Add(p);
                }
            }
        }
    }

    #endregion

}


