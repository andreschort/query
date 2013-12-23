// Register the namespace for the control.
Type.registerNamespace('QueryTables.Web');

//
// Define the control properties.
//
QueryTables.Web.DropDownFilter = function (element) {
    QueryTables.Web.DropDownFilter.initializeBase(this, [element]);
    
    this._hasFocus = null;
    this._postbackName = null;
    this._postbackParameter = null;
};

//
// Create the prototype for the control.
//

QueryTables.Web.DropDownFilter.prototype = {
    initialize: function() {
        QueryTables.Web.DropDownFilter.callBaseMethod(this, 'initialize');
        
        this._onchangeHandler = Function.createDelegate(this, this._onChange);

        $addHandler(this.get_element(), 'change', this._onChange, true);

        if (this._hasFocus) {
            this.get_element().focus();
        }
    },

    dispose: function() {
        QueryTables.Web.DropDownFilter.callBaseMethod(this, 'dispose');
    },

    //
    // Event delegates
    //
    
    _onChange: function (e) {
        var _this = $find(this.id);
        _this.doPostback();
    },

    //
    // Control properties
    //
    
    get_hasFocus: function () {
        return this._hasFocus;
    },

    set_hasFocus: function (value) {
        if (this._hasFocus !== value) {
            this._hasFocus = value;
            this.raisePropertyChanged('hasFocus');
        }
    },

    get_postbackName: function () {
        return this._postbackName;
    },

    set_postbackName: function (value) {
        if (this._postbackName !== value) {
            this._postbackName = value;
            this.raisePropertyChanged('postbackName');
        }
    },

    get_postbackParameter: function () {
        return this._postbackParameter;
    },

    set_postbackParameter: function (value) {
        if (this._postbackParameter !== value) {
            this._postbackParameter = value;
            this.raisePropertyChanged('postbackParameter');
        }
    },

    doPostback: function () {
        document.getElementById('__LASTFOCUS').value = this.get_element().name;
        __doPostBack(this._postbackName, this._postbackParameter);
    }
};

// Optional descriptor for JSON serialization.
QueryTables.Web.DropDownFilter.descriptor = {
    properties: [
        { name: 'hasFocus', type: Boolean },
        { name: 'postbackName', type: String },
        { name: 'postbackParameter', type: String },
    ]
};

// Register the class as a type that inherits from Sys.UI.Control.
QueryTables.Web.DropDownFilter.registerClass('QueryTables.Web.DropDownFilter', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();