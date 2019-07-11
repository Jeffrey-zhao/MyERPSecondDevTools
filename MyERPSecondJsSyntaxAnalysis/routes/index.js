var express = require('express');
var router = express.Router();
var db = require('./db');
var esprima = require('esprima');
estraverse = require('estraverse');
/* GET home page. */

//获取JS语法树数据
router.get('/', function (req, res, next) {
  //获取JS源码数据，转换为JS语法树结构数据
  async function getData(applicationId, callback) {
    const jsData = await db("select * from MyERPDevToolsScripts where ApplicationId='" + applicationId + "'");
    callback(jsData);
  }
  getData(req.param('applicationId'), function (jsData) {
    var jsTreeData = [];
    var updateStr = "";
    jsData.recordsets[0].forEach(element => {
      var esprimaTree = esprima.parseScript(element.JsData);
      var strEsprimaTree = JSON.stringify(esprimaTree);
      var moduleName = esprimaTree.body[0].expression.arguments[0].value;
      updateStr += " UPDATE dbo.MyERPDevToolsScripts SET JsModuleName='" + moduleName + "' WHERE ApplicationId='" + element.ApplicationId + "' AND JsName='" + element.JsName + "';";
      jsTreeData.push({
        JsPath: element.JsName,
        JsModuleName: esprimaTree.body[0].expression.arguments[0].value,
        JsSyntaxTreeJson: strEsprimaTree
      });
    });
    db(updateStr);
    res.send(jsTreeData);
  });
});

//获取指定方法的扩展方法列表
router.post('/GetPluginFunction', function (req, res, next) {
  //获取指定方法源码
  async function getData(param, callback) {
    var strSqlFilter = "";
    param.body.forEach(item => {
      strSqlFilter += " or JsModuleName = '" + item.moduleName + "'";
    });
    var strSql = "select * from MyERPDevToolsScripts where ApplicationId='" + param.applicationId + "' and (1=0 " + strSqlFilter + ")";
    const jsData = await db(strSql);
    if (jsData.recordset.length > 0)
      callback(jsData);
    else
      res.send([]);
  }

  getData(req.body, function (data) {
    var result = [];
    data.recordsets[0].forEach(element => {
      req.body.body.forEach(item => {
        var jsData = element.JsData;
        var ast = esprima.parseScript(jsData);
        var moduleName = ast.body[0].expression.arguments[0].value;
        if (moduleName == item.moduleName) {
          var resultItem = {
            pluginFunctions: []
          };
          resultItem.moduleName = item.moduleName;
          resultItem.functionName = item.functionName;
          var parentNode;
          var secondParentNode;
          var nodes = [];
          estraverse.traverse(ast, {
            enter: function (node, parent) {
              if (node.type == "Property" && node.key != null && node.key.name == "__module_plugins__")
                parentNode = node;
            }
          });
          if (parentNode == null) {
            return true;
          }
          estraverse.traverse(parentNode, {
            enter: function (node, parent) {
              if (node.type == "Property" && node.key != null && node.key.name == item.functionName) {
                secondParentNode = node;
                this.break();
              }
            }
          });
          if (secondParentNode == null) {
            return true;
          }
          estraverse.traverse(secondParentNode, {
            enter: function (node, parent) {
              if (node.type == "ObjectExpression" && parent != null && parent.type == "Property" && parent.kind == "init") {
                nodes.push(node);
              }
            }
          });
          nodes.forEach(element => {
            element.properties.forEach(item => {
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

//获取指定方法下对于AppService的引用
router.post('/GetAppServicesFunction', function (req, res, next) {
  //获取指定方法源码
  async function getData(param, callback) {
    var strSqlFilter = "";
    param.body.forEach(item => {
      strSqlFilter += " or JsModuleName = '" + item.moduleName + "'";
    });
    var strSql = "select * from MyERPDevToolsScripts where ApplicationId='" + param.applicationId + "' and (1=0 " + strSqlFilter + ")";
    const jsData = await db(strSql);
    if (jsData.recordset.length > 0)
      callback(jsData);
    else
      res.send([]);
  }

  getData(req.body, function (data) {
    var result = [];
    data.recordsets[0].forEach(element => {
      req.body.body.forEach(item => {
        var jsData = element.JsData;
        var ast = esprima.parseScript(jsData);
        var moduleName = ast.body[0].expression.arguments[0].value;
        if (moduleName == item.moduleName) {
          var resultItem = {
            appServiceFunctions: []
          };
          resultItem.moduleName = moduleName;
          resultItem.functionName = item.functionName;
          resultItem.pluginName = item.pluginName;
          var parentNode;
          var secondParentNode;
          var appServices = [];
          //获取定义的AppService变量名称
          estraverse.traverse(ast, {
            enter: function (node, parent) {
              if (node.type == "CallExpression" && node.callee != null && node.callee.name == "require") {
                if (node.arguments != null && node.arguments[0].value.indexOf('AppService') > 0) {
                  appServices.push({
                    appServiceName: node.arguments[0].value,
                    identifierAppServiceName: parent.id.name
                  });
                }
              }
            }
          });
          if (!appServices) {
            return true;
          }

          estraverse.traverse(ast, {
            enter: function (node, parent) {
              if (node.type == "Property" && node.key != null && node.key.name == item.functionName) {
                parentNode = node;
                this.break();
              }
            }
          });

          if (!parentNode)
            return true;

          if (item.pluginName) {
            estraverse.traverse(parentNode, {
              enter: function (node, parent) {
                if (node.type == "Property" && node.key.name == item.pluginName) {
                  secondParentNode = node;
                  this.break();
                }
              }
            });
          }

          if (!secondParentNode)
            secondParentNode = parentNode;

          estraverse.traverse(secondParentNode, {
            enter: function (node, parent) {
              if (node.type === 'CallExpression' && node.callee.type === 'MemberExpression') {
                var flag = false;
                var temp;
                for (var i = 0; i < appServices.length; i++) {
                  if (appServices[i].identifierAppServiceName == node.callee.object.name) {
                    flag = true;
                    temp = appServices[i];
                    break;
                  }
                }
                if (flag) {
                  resultItem.appServiceFunctions.push({
                    appServiceName: temp.appServiceName,
                    callName: temp.identifierAppServiceName,
                    functionName: node.callee.property.name
                  });
                }
              }
            }
          });
          if (resultItem.appServiceFunctions.length > 0)
            result.push(resultItem);
        }
      });
    });
    res.send(result);
  });
});

//获取指定模块下所有方法列表
router.post('/GetAllFunctionByModule', function (req, res, next) {
  //获取指定方法源码
  async function getData(param, callback) {
    var strSqlFilter = "";
    param.body.forEach(item => {
      strSqlFilter += " or JsModuleName = '" + item.moduleName + "'";
    });
    var strSql = "select * from MyERPDevToolsScripts where ApplicationId='" + param.applicationId + "' and (1=0 " + strSqlFilter + ")";
    const jsData = await db(strSql);
    if (jsData.recordset.length > 0)
      callback(jsData);
    else
      res.send([]);
  }

  getData(req.body, function (data) {
    var result = [];
    data.recordsets[0].forEach(element => {
      req.body.body.forEach(item => {
        var jsData = element.JsData;
        var ast = esprima.parseScript(jsData);
        var moduleName = ast.body[0].expression.arguments[0].value;
        if (moduleName == item.moduleName) {
          var resultItem = {
            moduleName: item.moduleName,
            functions: []
          };
          var parentNode;
          estraverse.traverse(ast, {
            enter: function (node, parent) {
              if (node.type == "Identifier" && node.name == "ns") {
                parentNode = parent;
                this.break();
              }
            }
          });
          if (parentNode == null) {
            return true;
          }
          parentNode.init.properties.forEach(element => {
            if (element.type == "Property" && element.value.type == "FunctionExpression") {
              var functionData = {
                type: "normal",
                name: element.key.name,
                params: []
              };
              if (element.value.params != null && element.value.params.length > 0) {
                element.value.params.forEach(eitem => {
                  functionData.params.push(eitem.name);
                })
              }
              resultItem.functions.push(functionData);
            }
          });

          if (item.moduleName.indexOf('Plugin') > 0) {
            var secondParentNode;
            estraverse.traverse(parentNode, {
              enter: function (node, parent) {
                if (node.type == "Property" && node.key != null && node.key.name == "__module_plugins__"){
                  secondParentNode = node;
                }
              }
            });
            if (secondParentNode == null) {
              return true;
            }
            var pluginMethodNode;
            estraverse.traverse(secondParentNode, {
              enter: function (node, parent) {
                if (node.type == "ObjectExpression" && parent != null && parent.type == "Property" && parent.kind == "init" && parent.key.name=="__module_plugins__") {
                  pluginMethodNode = node;
                  this.break();
                }
              }
            });
            if(pluginMethodNode != null && pluginMethodNode.properties != null && pluginMethodNode.properties.length > 0){
              pluginMethodNode.properties.forEach(pitem =>{
                var functionData = {
                  type: "plugin",
                  name: pitem.key.name,
                  params: []
                };
                resultItem.functions.push(functionData);
              });
            }
          }
          result.push(resultItem);
        }
      });
    });
    res.send(result);
  });
});

module.exports = router;
