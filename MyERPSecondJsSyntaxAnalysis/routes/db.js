var sqlserver = require('mssql');
var msdb = {};
var db = function(strsql) {
    sqlserver.close();
    return new Promise(function(resolve, reject){
        sqlserver.connect('mssql://sa:95938@wh-pc-lijq02/sql2014/LocalDataBase').then(function() {
        var req = new sqlserver.Request().query(strsql).then(function(recordset) {
            resolve(recordset)
        }).
        catch(function(err) {
            reject(err)
            sqlserver.close();
        });
    }).
    catch(function(err) {
        reject(err)
        sqlserver.close();
    });
    })
};
module.exports = db;