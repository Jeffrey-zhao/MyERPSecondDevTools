/*
* {{modulePluginName}} (.Plugin作为扩展模块的标识），扩展模块的最后一个参数$product二开产品合并后模块对象
*/

define("{{modulePluginName}}", function (require, exports, module, $product) {

    //var utility = require("utility");
    //var dialog = require("dialog");
    //var appService = require("Mysoft.xxx.xxx.AppService");

    /** 私有变量定义到这里
    var eventEnum = {
        //刷新机会列表
        OppReload: "Mysoft.Slxt.M00110616.BookingAdd.OppReload"
    }; 
    **/
    var ns = {
        /**Plugin模块中ns对象内不允许定义属性,避免和产品属性造成冲突**/
        /** 产品方法扩展区域**/
		//{{functionName}}扩展
        __module_plugins__: {
            {{functionName}}:{
				{{pluginType}}: function({{functionParams}}){
					
				}
			}
        }
    }

    module.exports = ns;
});
