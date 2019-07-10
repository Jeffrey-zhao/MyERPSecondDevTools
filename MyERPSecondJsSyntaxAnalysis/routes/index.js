var express = require('express');
var router = express.Router();
var db = require('./db');
var esprima = require('esprima');
estraverse = require('estraverse');
/* GET home page. */

router.get('/', function(req, res, next) {
  //获取JS源码数据，转换为JS语法树结构数据
  async function getData(applicationId, callback) {
    const jsData = await db("select * from MyERPDevToolsScripts where ApplicationId='"+applicationId+"'");
    callback(jsData);
  }
  getData(req.param('applicationId'), function(jsData){
    var jsTreeData = [];
    jsData.recordsets[0].forEach(element => {
      var esprimaTree = esprima.parseScript(element.JsData);
      var strEsprimaTree = JSON.stringify(esprimaTree);
      jsTreeData.push({
        JsName: element.JsName,
        JsSyntaxTreeJson: strEsprimaTree
      });
    });
    res.send(jsTreeData);
  });
});

router.get('/GetFunction', function(req, res, next) {
  //获取指定方法源码
  async function getData(applicationId, callback) {
    var jsName ="http://localhost:1005/Cbxt/PayMng/M02010404/HtfkApplyEdit.js?_t=78118841075798026&amp;lang=zh-CHS";
    const jsData = await db("select * from MyERPDevToolsScripts where ApplicationId='"+applicationId+"' and JsName = '"+jsName+"'");
    callback(jsData);
  }

  getData("92BDE040-F1F3-477A-8B8D-60365E9B017B", function(data){
    var jsData = data.recordsets[0][0].JsData;
    var ast = esprima.parseScript(jsData);
    var parentNode;
    var nodes = [];
    estraverse.traverse(ast, {
      enter: function (node, parent) {
        if(node.type == "Property" && node.key != null && node.key.name=="_appForm_btn_saveInApproving_click")
          parentNode = node;
      }
    });
    estraverse.traverse(parentNode, {
      enter: function (node, parent) {
        if(node.type=="CallExpression"){
          nodes.push(node);
          console.log(node);
        }
      }
    });
    res.send(nodes);
  });
});

//获取指定方法的扩展方法列表
router.post('/GetPluginFunction', function(req, res, next) {
  //获取指定方法源码
  async function getData(param, callback) {
    var strSqlFilter = "";
    param.body.forEach(item=>{
      strSqlFilter += " or JsName = '"+item.moduleName+"'";
    });
    var strSql = "select * from MyERPDevToolsScripts where ApplicationId='"+param.applicationId+"' and (1=0 "+strSqlFilter+")";
    const jsData = await db(strSql);
    if(jsData.recordset.length > 0)
      callback(jsData);
    else
      res.send([]);
  }

  getData(req.body, function(data){
    var result = [];
    data.recordsets[0].forEach(element => {
      req.body.body.forEach(item=>{
        if(element.JsName == item.moduleName)
        {
          var resultItem = {
            pluginFunctions: []
          };
          resultItem.moduleName = item.moduleName;
          resultItem.functionName = item.functionName;
          var jsData = element.JsData;
          var ast = esprima.parseScript(jsData);
          var parentNode;
          var secondParentNode;
          var nodes = [];
          estraverse.traverse(ast, {
            enter: function (node, parent) {
              if(node.type == "Property" && node.key != null && node.key.name=="__module_plugins__")
                parentNode = node;
            }
          });
          if(parentNode == null)
          {
            return true;
          }
          estraverse.traverse(parentNode, {
            enter: function (node, parent) {
              if(node.type=="Property" && node.key != null && node.key.name == item.functionName){
                secondParentNode = node;
                this.break();
              }
            }
          });
          if(secondParentNode == null)
          {
            return true;
          }
          estraverse.traverse(secondParentNode, {
            enter: function (node, parent) {
              if(node.type=="ObjectExpression" && parent != null && parent.type == "Property" && parent.kind == "init"){
                nodes.push(node);
              }
            }
          });
          nodes.forEach(element=>{
              element.properties.forEach(item=>{
                resultItem.pluginFunctions.push(item.key.name);
              });
          });
          result.push(resultItem);
        }
      });
    });
    res.send(result);
  });
});
module.exports = router;
