var sqlserver = require('mssql');
var msdb = {};
var db = function(strsql) {
    return new Promise(function(resolve, reject){
        sqlserver.connect('mssql://sa:95938@wh-pc-lijq02/sql2014/LocalDataBase').then(function() {
        var req = new sqlserver.Request().query(strsql).then(function(recordset) {
            resolve(recordset)
        }).
        catch(function(err) {
            reject(err)
        });
    }).
    catch(function(err) {
        reject(err)
    });
    })
};
module.exports = db;