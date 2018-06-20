var com = com || {};
(function ($) {

    if (!$) {
        return
    }

    com.apiurl = "/api/services/app";
    //ajax 这些后期不要了
    com.ajax = function (userOptions) {
        userOptions = userOptions || {};
        var options = $.extend({}, abp.ajax.defaultOpts, userOptions);
        options.success = undefined;
        options.error = undefined;
        var busyid = userOptions.loding || "body"; //异步加载的遮罩元素 id
        abp.ui.setBusy(busyid);
        abp.ajax(options).then(function (result) {
            abp.ui.clearBusy(busyid);
            if (result == null) {
                if (userOptions.success) {
                    userOptions.success(result);
                }
            }
            else {
                var meg = result.message || result.Message;
                if (result.success == false) {
                    abp.message.error("出现错误", meg);
                }
                else if (userOptions.success) {
                    userOptions.success(result);
                }
                else {
                    if (result.success || result.Success) {
                        abp.message.success("操作成功", meg);
                    }
                    else if (result.success == false || result.Success == false) {
                        abp.message.error("出现错误", meg);
                    }
                }
            }
        }).fail(function (result) {
            abp.ui.clearBusy(busyid);
            if (userOptions.error) {
                userOptions.error(result);
            }
        })
    };

    //消息 加载层
    com.success = function (title, meg) {
        abp.message.success(title, meg);
    }
    com.error = function (title, meg) {
        abp.message.error(title, meg);
    }

    com.confirm = function (title, callback) {
        abp.message.confirm(
            title,
            '您确定执行该操作吗?',
            function (isConfirmed) {
                if (isConfirmed)
                    callback();
            }
        );
    }

    com.delete = function (url, title, callback) {
        com.confirm(title, function () {
            com.ajax({
                url: url,
                success: function () {
                    callback();
                }
            });
        });
    }

    com.loading = function (t, busyid) {
        var _id = busyid || "body";
        if (t == false)
            abp.ui.clearBusy(_id);
        else
            abp.ui.setBusy(_id);
    }

    $.fn.extend({
        showModal: function (b) {
            if (b == false) {
                $(this).modal("hide");
            }
            else {
                $(this).modal();
            }
        }
    })
})(jQuery)


Array.prototype.RepeatBy = function (key, val) {
    var istr = false;
    this.forEach(function (item) {
        if (item[key] == val) {
            istr = true;
            return
        }
    });
    return istr;
};

Array.prototype.RepeatGetIndex = function (key, val) {
    var istr = -1;
    this.forEach(function (item, index) {
        if (item[key] == val) {
            istr = index;
            return istr;
        }
    });
    return istr;
};

Array.prototype.delete = function (key, val) {
    var index = -1;
    this.forEach(function (item, i) {
        if (item[key] == val)
            index = i;
    })
    this.splice(index, 1);
};

$(function () {

    $(".el-table tbody").off("click", "img").on("click", "img", function () {
        $('#_imgshowBig').show()
        $("#_imgshowBig_img").attr("src", $(this).attr("src"));
    })
})