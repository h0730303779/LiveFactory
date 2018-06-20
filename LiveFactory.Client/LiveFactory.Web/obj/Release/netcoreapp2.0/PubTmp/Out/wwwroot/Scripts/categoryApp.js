var categoryApp = new Vue({
    el: "#categoryApp",
    data: {
        categoryData: [],
        CategoryData: [],
        attrData: [],
        categoryAttrData: [],
        selectCategoryAttrData: [],
        searchParam: { KeyWord: "", SkipCount: 1, MaxResultCount: 50 },
        selectCategory: {},
        selectAttr: {},
        multipleSelection: [],//打勾选中的属性
        totalCount: 0,
        defaultProps: {
            children: 'childs',
            label: 'name'
        }
    },
    methods: {
        getCategoryData: function () {
            com.loading(true);
            com.api.category.getList().done(function (result) {
                com.loading(false);
                categoryApp.categoryData = result.items;
            });
        },
        addCategory: function () {
            this.selectCategory = {};
            $('#editCategory').showModal();
        },
        editModel: function () {
            var item = this.$refs.categoryTree.getCurrentNode();
            if (item == null) {
                com.error("请选择分类进行修改")
                return;
            }
            this.selectCategory = item;
            $('#editCategory').showModal();
        },
        delCategory: function () {
            var item = this.$refs.categoryTree.getCurrentNode();
            if (item == null) {
                com.error("请选择分类进行修改")
                return;
            }

            com.confirm("确定要删除吗?", function () {
                com.loading();
                com.api.category.delete(item.id).done(function () {
                    com.loading(false);
                    categoryApp.getCategoryData();
                })
            });
        },
        selectCategoryChange: function (item, item2) {

            //分类选中事件
            this.selectCategory = item;
            this.categoryAttrData = [];
            this.getCategortArrtData();
        },
        getCategortArrtData: function () {
            com.loading();
            com.api.attr.getListByCategoryid(this.selectCategory.id).done(function (result) {
                com.loading(false);
                categoryApp.categoryAttrData = result.items;
            });
        },
        selectCategoryAttrChange: function (currentRow, oldCurrentRow) {
            //分类属性选中事件
            this.selectCategoryAttrData = currentRow == null ? [] : currentRow.attrValue;
        },

        //获取基础属性数据
        getAddSelectAttr: function () {
            com.loading();
            this.searchParam["id"] = this.selectCategory.id;
            com.api.attr.getListNoInSelectByCategoryid(this.searchParam).done(function (result) {
                com.loading(false);
                categoryApp.attrData = result.items;
                categoryApp.totalCount = result.totalCount;
            });
        },
        addSelectAttr: function () {
            //选择添加属性
            $('#selectAttr').showModal();
            this.getAddSelectAttr();
        },
        selectAttrChange: function (currentRow, oldCurrentRow) {
            //属性数据选中数据
            this.selectAttr = currentRow == null ? {} : currentRow;
        },
        SelectionChange1: function (val) {
            //属性数据 table checked 选中的事件
            val.forEach(function (item) {
                categoryApp.multipleSelection.push({ id: item.id, name: item.name, attrValue: [] });
            });
        },
        SelectionChange2: function (val) {
            //属性值数据 table checked 选中的事件
            var index = this.multipleSelection.RepeatGetIndex("id", this.selectAttr.id);
            if (index > -1)
                this.multipleSelection[index].attrValue = val;
        },
        //提交保存选中方法
        saveAddSelectAttr: function () {
            com.loading(true, "#selectAttr")
            com.api.attr.saveCategoryAttrValue({ CategoryId: this.selectCategory.id, Attr: this.multipleSelection }).done(function () {
                $('#selectAttr').showModal(false);
                com.loading(false, "#selectAttr");
                categoryApp.getCategortArrtData();
            });
        },
        delCategoryAttr: function (item) {
            var scope = this;
            com.confirm("确定要删除吗?", function () {
                com.loading();
                var valid = item.valueId ? item.valueId : null;
                com.api.attr.deleteCategoryAttrValuee(scope.selectCategory.id, item.id).done(function () {
                    com.loading(false);
                    categoryApp.getCategortArrtData();
                })
            });
        }
    }
});
$(function () {
    categoryApp.getCategoryData();
    $("#editCategoryForm").submit(function (e) {
        e.preventDefault();
        if (!$("#editCategoryForm").valid()) {
            return false;
        }
        com.loading(true, "#editCategoryForm");
        com.api.category.update(categoryApp.selectCategory).done(function (res) {
            categoryApp.getCategoryData();
            $('#editCategory').showModal(false);
        }).always(function () { com.loading(false, "#editCategoryForm"); });
    })

})

//Array.prototype.AttrRepert = function () {
//    this.forEach(function (item) {
//        var index = categoryApp.categoryAttrData.RepeatGetIndex("id", item.id);
//        if (index > -1) {
//            item.attrValue.forEach(function (item2) {
//                if (!categoryApp.categoryAttrData[index].attrValue.RepeatBy("id", item2.id)) {
//                    categoryApp.categoryAttrData[index].attrValue.push(item2);
//                }
//            });
//        }
//        else {
//            categoryApp.categoryAttrData.push(item);
//        }
//    });
//}