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

            var showing = function (sender, args) { _this.cancelShowWhenFocus(sender, args); };
            var selection = function (sender, arg) { _this.doPostback(); };
            
            calendar.add_showing(showing);

            calendar.add_dateSelectionChanged(selection);
        }, 1);
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

    // Hide the calendar popup after filtering
    cancelShowWhenFocus: function (sender, args) {
        if (this._showWasCancelled && !this._showWasCancelled2 && document.selection) {
            // Some times the calendar is initialized twice (IE only)
            this._showWasCancelled2 = true;
            args.set_cancel(true);
        }
        else if (this._hasFocus && !this._showWasCancelled) {
            this._showWasCancelled = true;
            args.set_cancel(true);
            var _this = this;
            // If IE does not initialize the calendar twice
            setTimeout(function () { _this._showWasCancelled2 = true; }, 100);
        }
    }
};

// Optional descriptor for JSON serialization.
QueryTables.Web.DateFilter.descriptor = {
    properties: [
        { name: 'calendarExtenderId', type: String },
        { name: 'showWasCancelled', type: Boolean }
    ]
};

// Register the class as a type that inherits from Sys.UI.Control.
QueryTables.Web.DateFilter.registerClass('QueryTables.Web.DateFilter', QueryTables.Web.TextFilter);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();