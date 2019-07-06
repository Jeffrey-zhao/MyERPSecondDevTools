var express = require('express');
var router = express.Router();
var db = require('./db');
var esprima = require('esprima');

let jsTreeData = [];

async function getData() {
    const jsData = await db('select * from MyERPDevToolsScripts');
    jsData.recordsets[0].forEach(element => {
      var esprimaTree = esprima.parseScript(element.JsData);
      var strEsprimaTree = JSON.stringify(esprimaTree);
      jsTreeData.push({
        jsName: element.JsName,
        strEsprimaTree: strEsprimaTree
      });
    });
}

getData();

/* GET home page. */

router.get('/', function(req, res, next) {
  res.render('index', { rdata: jsTreeData });
});

module.exports = router;
