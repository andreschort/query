function setFocus(element) {
    var elemLen = element.value.length;
    // For IE Only
    if (document.selection) {
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
    else if (element.selectionStart || element.selectionStart == '0') {
        // Firefox/Chrome
        element.selectionStart = elemLen;
        element.selectionEnd = elemLen;
        element.focus();
    } // if
} // SetCaretAtEnd()

function initTextFilter($element) {
    var currentVal = $element.val();
    var delayMs = $element.attr('data-query-filterDelay');
    var postbackName = $element.attr('data-query-postbackName');
    var focus = $element.attr('data-query-focus');
    
    // Store initial value ignoring the default (placeholder)
    if (currentVal == defaultValue) {
        currentVal = "";
    }
    $element.attr("data-query-value", currentVal);
    
    // Restore focus
    focus = focus ? focus.toLowerCase() : 'false';
    if (focus == 'true') {
        setFocus($element[0]);
    }

    $element.keyup(function (e) {
        // If the user pressed the ENTER key (13) make a postback
        var theKey;
        e = (window.event) ? event : e;
        theKey = (e.keyCode) ? e.keyCode : e.charCode;

        document.getElementById('__LASTFOCUS').value = this.name;
        if (theKey == "13") { // 13 ENTER key
            __doPostBack(postbackName, '');
        }
        
        // If no delay was defined we are done
        if (delayMs == undefined) {
            return;
        }

        // Set timer for auto postback
        var oldVal = $(this).attr('data-query-value');  
        var val = $(this).val();

        // Ignore default value (placeholder)
        if (val == defaultValue) {
            val = "";
        }

        // skip postback if value did not change
        if (oldVal != val) {
            $(this).attr('data-query-value', val);
            delay(function () {
                __doPostBack(postbackName, '');
            }, delayMs);
        }
    });
}

var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();

function initDateFilter($element) {
    initTextFilter($element);
    
    var focus = $element.attr('data-query-focus');
    var datepickerBackoff = focus ? focus.toLowerCase() : 'false';
    
    // set timeout to disable datepickerBackoff in case it is not disabled by the beforeShow. (needed if browser is not IE)
    setTimeout(function () { datepickerBackoff = 'false'; }, 10);
    $element.datepicker({
        onSelect: function () {
            document.getElementById('__LASTFOCUS').value = this.name;
            __doPostBack($(this).attr('data-query-postbackName'), '');
        },
        beforeShow: function (input, inst) {
            // FIX IE: Do not show datepicker dialog after postback from this filter
            if (datepickerBackoff == 'true') {
                datepickerBackoff = 'false';
                return false;
            }

            return true;
        }
    });
}

function initDropDownFilter($element) {
    var focus = $element.attr('data-query-focus');
    // Restore focus
    focus = focus ? focus.toLowerCase() : 'false';
    if (focus == 'true') {
        $element[0].focus();
    }
    
    $element.change(function() {
        document.getElementById('__LASTFOCUS').value = this.name;
        __doPostBack($(this).attr('data-query-postbackName'), '');
    });
}