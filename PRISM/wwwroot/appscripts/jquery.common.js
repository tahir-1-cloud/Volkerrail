$(document).ajaxComplete(
    function (event, xhr, settings) {
        if (xhr.status == 401) {
            window.location.href = "";
        }

    });

$(document).ready(function () {
    if (typeof $.fn.numeric !== 'undefined') {
        $(".number").numeric();
    }
});


var Common = function () {
   
    _this = this;

    _this.AjaxCall = function (url, data, methodType, isAsync, callback, btn) {
        var value = $(btn).val();
        if (value == "") {
            value = $(btn).html();
        }
        $(btn).val("Sss...").prop("disabled", true); // for input element
        $(btn).width($(btn).width()).html("<i class='fa fa-circle-o-notch fa-spin fa-fw'></i>").prop("disabled", true); // for button element
        $.ajax({
            type: methodType,
            dataType: 'json',
            crossDomain: true,
            contentType: 'application/json; charset=utf-8', // text for IE, xml for the rest ,
            url: url,
            data: data,
            async: isAsync,
            success: function (response) {
                $(btn).val(value).prop("disabled", false); // for input element
                $(btn).removeAttr("style").html(value).prop("disabled", false); // for button element

                if (response.IsValid == false && response.Message == "LoggedOut") {
                    _this.ShowMessage("You are logged out.", "error");
                    setTimeout(function () {
                        window.location = "";
                    }, 1000);
                }
                else {
                    if (response.IsValid == false && response.ShowMessage == true) {
                        _this.ShowMessage(response.Message, "error");
                    }
                }

                callback(response);
            },
            error: function (jqXhr, textStatus, errorThrown) {
                $(btn).val(value).prop("disabled", false); // for input element
                $(btn).removeAttr("style").html(value).prop("disabled", false); // for button element

                if (jqXhr.getResponseHeader('Content-Type').indexOf('application/json') > -1) {
                    // only parse the response if you know it is JSON
                    var error = $.parseJSON(jqXhr.responseText);
                    _this.ShowMessage(error, "error");
                } else {
                   
                    _this.ShowMessage("Oops! Something went wrong, please try again later.", "error");
                }
                //$(".modal").modal("hide");
            }
        });
    }
    _this.ConfirmAjaxCall = function (url, data, methodType, isAsync, callback, btn, txt) {
        if (txt != "" && txt != null) {
            $("#txtGlobal").text(txt);
        }
        else {
            $("#txtGlobal").text("Are you sure that you want to delete?");
        }

        $("#myGlobalModal").modal("show");

        $("#btnGlobalConfirm").click(function () {
            _this.AjaxCall(url, data, methodType, isAsync, callback, btn);
            $("#myGlobalModal").modal("hide");
        });
    }
    _this.AjaxCallFormData = function (url, data, isAsync, callback, btn) {
        var value = $(btn).val();
        if (value == "") {
            value = $(btn).html();
        }
        $(btn).val("Processing...").prop("disabled", true); // for input element
        $(btn).width($(btn).width()).html("<i class='fa fa-circle-o-notch fa-spin fa-fw'></i>").prop("disabled", true); // for button element

        $.ajax({
            url:  url,
            data: data,
            contentType: false,
            processData: false,
            async: isAsync,
            type: 'POST',
            success: function (response) {
                $(btn).val(value).prop("disabled", false); // for input element
                $(btn).removeAttr("style").html(value).prop("disabled", false); // for button element

                if (response.IsValid == false && response.Message == "LoggedOut") {
                    _this.ShowMessage("You are logged out.", "error");
                    setTimeout(function () {
                        window.location = "https://localhost:44390/";
                    }, 1000);
                }
                else {
                    if (response.IsValid == false && response.ShowMessage == true) {
                        _this.ShowMessage(response.Message, "error");
                    }
                }

                callback(response);
            },
            error: function (jqXhr, textStatus, errorThrown) {
                
                $(btn).val(value).prop("disabled", false); // for input element
                $(btn).removeAttr("style").html(value).prop("disabled", false); // for button element

                if (jqXhr.getResponseHeader('Content-Type').indexOf('application/json') > -1) {
                    // only parse the response if you know it is JSON
                    var error = $.parseJSON(jqXhr.responseText);
                    _this.ShowMessage(error, "error");
                } else {
                    _this.ShowMessage("Oops! Something went wrong, please try again later.", "error");
                }
                //$(".modal").modal("hide");
            }
        });
    }
    _this.Confirm = function (btn, txt) {
        if (txt != "" && txt != null) {
            $("#txtGlobal").text(txt);
        }
        else {
            $("#txtGlobal").text("Are you sure that you want to delete?");
        }
        $("#myGlobalModal").modal("show");
        $("#btnGlobalConfirm").click(function () {
            $(btn).trigger("click");
            $("#myGlobalModal").modal("hide");
            return true;
        });
    }
    _this.ShowMessage = function (msg, type) {
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": true,
            "progressBar": false,
            "positionClass": "toast-top-right",
            "preventDuplicates": true,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "10000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        }
        toastr[type](msg, type.charAt(0).toUpperCase() + type.slice(1))
    }
    _this.Validate = function (form) {
        // Validation
        if ($(form).length > 0) {
            if (!$(form).validationEngine('validate',
                {
                scroll: true,
                promptPosition: "bottomLeft",
                autoHidePrompt: true
            })) {
                return false;
            }
            else {
                return true;
            }
        }
    }
    _this.ValidateParsley = function (form) {
        // Validation
        if ($(form).length > 0) {
            $(form).parsley().validate();
            if ($('.parsley-error').length === 0) {
                return true;
            }
            else {
                return false;
            }
            //.on('field:validated', function () {
            //    var ok = $('.parsley-error').length === 0;
            //    return ok;
            //});
        }
    }
    _this.GetFormValues = function (form) {
        var json = {};
        $(form).find("input, select, textarea").not("input[type=radio]").each(function () {
            if ($(this).is("[name]")) {
                if ($(this).is(':checkbox')) {
                    json[$(this).attr("name")] = $(this).is(":checked");
                }
                else {
                    json[$(this).attr("name")] = $(this).val();
                }
            }

        });
        return json;
    }
    _this.ClearFormValues = function (form) {
        var json = {};
        $(form).find("input[type=text], select, textarea, input[type=hidden]").each(function () {
            if ($(this).is("[data-element]")) {
                json[$(this).attr("data-element")] = $(this).val("");
            }
        });
        return json;
    }
    _this.ClearAddHTML = function (id, html) {

        $('#' + id).html('');
        $('#' + id).html(html);

    }
    _this.GetQueryStringParams = function (sParam) {
        var sPageURL = window.location.search.substring(1);
        var sURLVariables = sPageURL.split('&');
        for (var i = 0; i < sURLVariables.length; i++) {
            var sParameterName = sURLVariables[i].split('=');
            if (sParameterName[0] == sParam) {
                return sParameterName[1];
            }
        }
    }
    _this.GetFirstQueryStringParams = function (sParam) {
        var sPageURL = window.location.search.substring(1);
        var sURLVariables = sPageURL.split('&');

        for (var i = 0; i < sURLVariables.length; i++) {
            var sParameterName = sURLVariables[0].split('d=');

            if (sParameterName[0] + 'd' == sParam) {
                return sParameterName[1];
            }
        }
    }
    _this.CommaNumber = function (num) {
        return Number(num).toLocaleString('en');
    }
    _this.readImage = function (id) {
        if (this.files && this.files[0]) {
            var FR = new FileReader();
            FR.onload = function (e) {
                $('#' + id).attr("src", e.target.result);
                //$("#base64").val(e.target.result);
            };
            FR.readAsDataURL(this.files[0]);
        }
    }
    _this.readImage = function (file, target, callback) {
        if (file[0].files && file[0].files[0]) {
            var FR = new FileReader();
            FR.onload = function (e) {
                target.attr("src", e.target.result);
                callback(true);
            };
            FR.readAsDataURL(file[0].files[0]);
        }
    }
    _this.isValidPassword = function (input) {
        var reg = /^[^%\s]{6,}$/;
        var reg2 = /[a-zA-Z]/;
        var reg3 = /[0-9]/;
        return reg.test(input) && reg2.test(input) && reg3.test(input);
    }
    _this.copyToClipboard = function (elem) {
        // create hidden text element, if it doesn't already exist
        var targetId = "_hiddenCopyText_";
        var isInput = elem.tagName === "INPUT" || elem.tagName === "TEXTAREA";
        var origSelectionStart, origSelectionEnd;
        if (isInput) {
            // can just use the original source element for the selection and copy
            target = elem;
            origSelectionStart = elem.selectionStart;
            origSelectionEnd = elem.selectionEnd;
        } else {
            // must use a temporary form element for the selection and copy
            target = document.getElementById(targetId);
            if (!target) {
                var target = document.createElement("textarea");
                target.style.position = "absolute";
                target.style.left = "-9999px";
                target.style.top = "0";
                target.id = targetId;
                document.body.appendChild(target);
            }
            target.textContent = elem.textContent;
        }
        // select the content
        var currentFocus = document.activeElement;
        target.focus();
        target.setSelectionRange(0, target.value.length);

        // copy the selection
        var succeed;
        try {
            succeed = document.execCommand("copy");
        } catch (e) {
            succeed = false;
        }
        // restore original focus
        if (currentFocus && typeof currentFocus.focus === "function") {
            currentFocus.focus();
        }

        if (isInput) {
            // restore prior selection
            elem.setSelectionRange(origSelectionStart, origSelectionEnd);
        } else {
            // clear temporary content
            target.textContent = "";
        }
        return succeed;
    }
    _this.GetURLParameter = function (url) {
        var sPageURL = url;
        //var sPageURL = window.location.href;
        var indexOfLastSlash = sPageURL.lastIndexOf("/");

        if (indexOfLastSlash > 0 && sPageURL.length - 1 != indexOfLastSlash)
            return sPageURL.substring(indexOfLastSlash + 1);
        else
            return 0;

    }
    _this.formatAMPM = function (date) {
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }
    _this.EncodeBase64 = function ($str) {

        var encodedString = Base64.encode($str);
        return encodedString;

    }
    _this.DecodeBase64 = function ($str) {
        var decodedString = Base64.decode($str);
        return decodedString;
    }
    _this.validateEmail = function (email) {
        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }
}

var orderStatus = {
    Pending: "Pending",
    Cooking: "Cooking",
    Cooked: "Prepared",
    Completed: "Completed"
}
var Base64 = {


    _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",


    encode: function (input) {
        var output = "";
        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
        var i = 0;

        input = Base64._utf8_encode(input);

        while (i < input.length) {

            chr1 = input.charCodeAt(i++);
            chr2 = input.charCodeAt(i++);
            chr3 = input.charCodeAt(i++);

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            } else if (isNaN(chr3)) {
                enc4 = 64;
            }

            output = output + this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) + this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);

        }

        return output;
    },


    decode: function (input) {
        var output = "";
        var chr1, chr2, chr3;
        var enc1, enc2, enc3, enc4;
        var i = 0;

        input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

        while (i < input.length) {

            enc1 = this._keyStr.indexOf(input.charAt(i++));
            enc2 = this._keyStr.indexOf(input.charAt(i++));
            enc3 = this._keyStr.indexOf(input.charAt(i++));
            enc4 = this._keyStr.indexOf(input.charAt(i++));

            chr1 = (enc1 << 2) | (enc2 >> 4);
            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
            chr3 = ((enc3 & 3) << 6) | enc4;

            output = output + String.fromCharCode(chr1);

            if (enc3 != 64) {
                output = output + String.fromCharCode(chr2);
            }
            if (enc4 != 64) {
                output = output + String.fromCharCode(chr3);
            }

        }

        output = Base64._utf8_decode(output);

        return output;

    },

    _utf8_encode: function (string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";

        for (var n = 0; n < string.length; n++) {

            var c = string.charCodeAt(n);

            if (c < 128) {
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }

        }

        return utftext;
    },

    _utf8_decode: function (utftext) {
        var string = "";
        var i = 0;
        var c = c1 = c2 = 0;

        while (i < utftext.length) {

            c = utftext.charCodeAt(i);

            if (c < 128) {
                string += String.fromCharCode(c);
                i++;
            }
            else if ((c > 191) && (c < 224)) {
                c2 = utftext.charCodeAt(i + 1);
                string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                i += 2;
            }
            else {
                c2 = utftext.charCodeAt(i + 1);
                c3 = utftext.charCodeAt(i + 2);
                string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                i += 3;
            }

        }

        return string;
    }

}





