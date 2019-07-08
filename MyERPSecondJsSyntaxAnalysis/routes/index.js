var express = require('express');
var router = express.Router();
var db = require('./db');
var esprima = require('esprima');
let jsTreeData = [];

/* GET home page. */

router.get('/', function(req, res, next) {
  //获取JS源码数据，转换为JS语法树结构数据
  async function getData(applicationId) {
    const jsData = await db("select * from MyERPDevToolsScripts where ApplicationId='"+applicationId+"'");
    jsData.recordsets[0].forEach(element => {
      var esprimaTree = esprima.parseScript(element.JsData);
      var strEsprimaTree = JSON.stringify(esprimaTree);
      jsTreeData.push({
        jsName: element.JsName,
        strEsprimaTree: strEsprimaTree
      });
    });
  }

  getData(req.param('applicationId'));
  res.render('index', { rdata: jsTreeData });
});

module.exports = router;
