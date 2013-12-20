// Register the namespace for the control.
Type.registerNamespace('QueryTables.Web');

//
// Define the control properties.
//
QueryTables.Web.DateFilter = function (element) {
    QueryTables.Web.DateFilter.initializeBase(this, [element]);
    
    this._calendarExtenderId = null;
    this._showWasCancelled = null;
};

//
// Create the prototype for the control.
//

QueryTables.Web.DateFilter.prototype = {
    initialize: function() {
        QueryTables.Web.DateFilter.callBaseMethod(this, 'initialize');
        var _this = this;
        setTimeout(function () {
            var calendar = $find(_this._calendarExtenderId);

            calendar.add_showing(function (sender, args) { _this.cancelShowWhenFocus(_this, sender, args); });
            calendar.add_dateSelectionChanged(function(sender, arg) { _this.doPostback(); });
        }, 10);
    },

    dispose: function() {
        QueryTables.Web.DateFilter.callBaseMethod(this, 'dispose');
    },

    //
    // Event delegates
    //

    //
    // Control properties
    //

    get_calendarExtenderId: function () {
        return this._calendarExtenderId;
    },

    set_calendarExtenderId: function (value) {
        if (this._calendarExtenderId !== value) {
            this._calendarExtenderId = value;
            this.raisePropertyChanged('calendarExtenderId');
        }
    },

    cancelShowWhenFocus: function (_this, sender, args) {
        console.log('cancelShowWhenFocus: ' + _this._showWasCancelled);
        if (_this._showWasCancelled) {
            console.log('cancelShowWhenFocus: return');
            return;
        }
        
        if (_this._hasFocus) {
            console.log('cancelShowWhenFocus: ' + _this._hasFocus);
            _this._showWasCancelled = true;
            args.set_cancel(true);
        }
    }
};

// Optional descriptor for JSON serialization.
QueryTables.Web.DateFilter.descriptor = {
    properties: [
        { name: 'calendarExtenderId', type: String },
        { name: 'showWasCancelled', type: Boolean },
    ]
};

// Register the class as a type that inherits from Sys.UI.Control.
QueryTables.Web.DateFilter.registerClass('QueryTables.Web.DateFilter', QueryTables.Web.TextFilter);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();