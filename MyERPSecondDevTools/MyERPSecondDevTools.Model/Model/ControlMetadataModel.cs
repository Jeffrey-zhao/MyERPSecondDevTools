using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Model.Model
{
    public class SyncTip
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> names { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isTip { get; set; }
    }

    public class Command
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string queryDb { get; set; }
    }

    public class FieldsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allowPopulate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string entity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string storeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sourceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string entityAlias { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> fields { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string usedType { get; set; }
    }

    public class AvailableFieldItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isRedundance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isPrimaryAttribute { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string aliasName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string entity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int entityType { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string attributeType { get; set; }
    }

    public class View
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 合同资源化授权
        /// </summary>
        public string displayName { get; set; }
    }

    public class ResourceFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public View view { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> actions { get; set; }
    }

    public class FieldsItem2
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 调整后合同净值
        /// </summary>
        public string displayName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dataType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string field { get; set; }
    }

    public class DiagramsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string primaryField { get; set; }
        /// <summary>
        /// 合同
        /// </summary>
        public string displayName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string projFilter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int projectFilterType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string projectFilterName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string resFilter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isMaster { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int joinType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string logicFormula { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int conditionType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> conditions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ResourceFilter resourceFilter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string diagramRelation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isAllowProjFilter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isAllowRsFilter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FieldsItem2> fields { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
    }

    public class DataSource
    {
        /// <summary>
        /// 
        /// </summary>
        public string keyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string entity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string withNoLock { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int mode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Command command { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FieldsItem> fields { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<AvailableFieldItem> availableField { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> fixedSortings { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> summaries { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DiagramsItem> diagrams { get; set; }
    }

    public class CellStylesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string labelWidth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string width { get; set; }
    }

    public class Label
    {
        /// <summary>
        /// 合同名称
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string visible { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isMoreCondition { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string align { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tips { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string conditionType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string requirementLevel { get; set; }
    }

    public class Control
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isBold { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int maxLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string iconClass { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string extraHtml { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string controlName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string errorMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string readonlyMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string defaultValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string requirementLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string templateStyle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string placeholder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isHidden { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> customProps { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string format { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> events { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
    }

    public class Column
    {
        /// <summary>
        /// 
        /// </summary>
        public string width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allowEdit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isHidden { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fontColor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string enableRollUp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> customProps { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string exportIgnore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dataType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string align { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dataSourceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isRedundantField { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Control control { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string template { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string allowSort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string defaultSort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isSummaryColumn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isBold { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string textField { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tips { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string subControlFormula { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> behaviors { get; set; }
        /// <summary>
        /// 合同名称
        /// </summary>
        public string title { get; set; }
    }

    public class CellsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int colSpan { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int rowSpan { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Label label { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Column column { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MetadataKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
    }

    public class RowsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CellsItem> cells { get; set; }
    }

    public class @groupItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 合同基本信息
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string disableStyle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isHidden { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CellStylesItem> cellStyles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<RowsItem> rows { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
    }

    public class RegionsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string regionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 基本信息
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tabTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string disableStyle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isHidden { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> events { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<@groupItem> @group { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
    }

    public class GroupsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string align { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> items { get; set; }
    }

    public class ToolbarsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string toolbarId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string templateStyle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<GroupsItem> groups { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> events { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string quickFinds { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string filter { get; set; }
    }

    public class EventsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string functionName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string enabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
    }

    public class HiddensItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string allowPopulate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isCustomField { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string controlName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string errorMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string readonlyMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string defaultValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string requirementLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string templateStyle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string placeholder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isHidden { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> customProps { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string format { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> events { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
    }

    public class HandlesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string handleId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ruleId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string action { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
    }

    public class ConfigsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 保存
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string controlId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string controlType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string controlSubType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string controlProp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string controlName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<HandlesItem> handles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
    }

    public class GroupsItem2
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 成本归集隐藏
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rule { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
    }

    public class Rule
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ConfigsItem> configs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<GroupsItem2> groups { get; set; }
    }

    public class Layout
    {
        /// <summary>
        /// 
        /// </summary>
        public string methodName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string serviceName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string concurrencyDetect { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string doubleToolBar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string asyncRender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string templateStyle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string showNavigation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<RegionsItem> regions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ToolbarsItem> toolbars { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EventsItem> events { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> attributes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<HiddensItem> hiddens { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> langs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Rule rule { get; set; }
    }

    public class Item
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string formId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isSyncAvailableFields { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SyncTip syncTip { get; set; }
        /// <summary>
        /// 合同表单控件
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string controlHandler { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string regionDisplayRule { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string entityId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DataSource dataSource { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Layout layout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> checkRules { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string functionPageId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int isRevisedId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string application { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string controlName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataversion { get; set; }
        /// <summary>
        /// 系+先生+
        /// </summary>
        public string createdBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string createdOn { get; set; }
        /// <summary>
        /// 系统管理员
        /// </summary>
        public string modifiedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string modifiedOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string metadataStatus { get; set; }
    }

    public class ControlMetadataModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Item item { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
    }
}
