// Register the namespace for the control.
Type.registerNamespace('QueryTables.Web');

//
// Define the control properties.
//
QueryTables.Web.TextFilter = function (element) {
    QueryTables.Web.TextFilter.initializeBase(this, [element]);

    this._placeholder = null;
    this._autoFilterDelay = null;
    this._hasFocus = null;
    this._postbackName = null;
    this._postbackParameter = null;
    this._currentValue = element.value;
    this._timer = 0;
};

//
// Create the prototype for the control.
//

QueryTables.Web.TextFilter.prototype = {
    initialize: function() {
        QueryTables.Web.TextFilter.callBaseMethod(this, 'initialize');

        var element = this.get_element();

        this._onkeyupHandler = Function.createDelegate(this, this._onKeyUp);

        $addHandler(element, 'keyup', this._onKeyUp, true);

        // Restore focus
        if (this._hasFocus) {
            var _this = this;
            // use timeout to wait for every element to be loaded
            setTimeout(function() { _this.setFocus(); }, 10);
        }
    },

    dispose: function() {
        QueryTables.Web.TextFilter.callBaseMethod(this, 'dispose');
    },

    //
    // Event delegates
    //

    _onKeyUp: function(e) {
        // If the user pressed the ENTER key (13) make a postback
        var theKey;
        e = (window.event) ? event : e;
        theKey = (e.keyCode) ? e.keyCode : e.charCode;

        var _this = $find(this.id);

        if (theKey == "13") { // 13 ENTER key
            _this.doPostback();
        } else {
            _this.schedulePostback();
        }
    },


    //
    // Control properties
    //
    
    get_placeholder: function () {
        return this._placeholder;
    },

    set_placeholder: function (value) {
        if (this._placeholder !== value) {
            this._placeholder = value;
            this.raisePropertyChanged('placeholder');
        }
    },

    get_autoFilterDelay: function () {
        return this._autoFilterDelay;
    },

    set_autoFilterDelay: function (value) {
        if (this._autoFilterDelay !== value) {
            this._autoFilterDelay = value;
            this.raisePropertyChanged('autoFilterDelay');
        }
    },

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
    
    doPostback: function() {
        clearTimeout(this._timer);
        document.getElementById('__LASTFOCUS').value = this.get_element().name;
        __doPostBack(this._postbackName, this._postbackParameter);
    },
    
    schedulePostback: function() {
        // If no delay was defined we are done
        if (this._autoFilterDelay === undefined) {
            return;
        }

        // Set timer for auto postback
        var value = this.get_element().value;
        
        // skip postback if value did not change
        if (this._currentValue != value) {
            this._currentValue = value;
            clearTimeout(this._timer);
            var _this = this;
            this._timer = setTimeout(function () { _this.doPostback(); }, this._autoFilterDelay);
        }
    },
    
    setFocus: function () {
        try {
            var element = this.get_element();
            var elemLen = element.value.length;
            if (document.selection) { // IE
                // Set focus
                element.focus();
                // Use IE Ranges
                var oSel = document.selection.createRange();
                // Reset position to 0 & then set at end
                oSel.moveStart('character', -elemLen);
                oSel.moveStart('character', elemLen);
                oSel.moveEnd('character', 0);
                oSel.select();
            }
            else if (element.selectionStart || element.selectionStart == '0') { // Firefox/Chrome
                element.selectionStart = elemLen;
                element.selectionEnd = elemLen;
                element.focus();
            }
        } catch (e) {
            window.console && console.log && console.log("TextFilter: setFocus:", e);
        }
    }
};

// Optional descriptor for JSON serialization.
QueryTables.Web.TextFilter.descriptor = {
    properties: [
        { name: 'placeholder', type: String },
        { name: 'autoFilterDelay', type: Number },
        { name: 'hasFocus', type: Boolean },
        { name: 'postbackName', type: String },
        { name: 'postbackParameter', type: String }
    ]
};

// Register the class as a type that inherits from Sys.UI.Control.
QueryTables.Web.TextFilter.registerClass('QueryTables.Web.TextFilter', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();