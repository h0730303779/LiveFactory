

//产品分类
Vue.component("product-category", {
    template: `<el-select :value="value" clearable @change="_selectChange"  placeholder="请选择">
   <template  v-for="item in categorydata">
 <el-option
      :key="item.id"
      :label="item.name"
      :value="item.id">
    </el-option>
 <el-option v-for="child in item.childs"
      :key="child.id"
      :label="child.name"
      :value="child.id">
 <span style="float: left;padding-left:20px;"> |--{{child.name }}</span>
    </el-option>
</template>
  </el-select>`,
    props: ["value"],
    data: function () {
        return { categorydata: [] };
    },
    methods: {
        getcategory: function () {
            var _soce = this;
            com.api.category.getList().done(function (result) {
                _soce.categorydata = result.items;
            });
        },
        _selectChange: function (value) {
            var formattedValue = value
            // 通过 input 事件带出数值
            this.$emit('input', formattedValue)
        }
    },
    created: function () {
        this.getcategory();
    }
})

Vue.component("play-type", {
    template: `<el-select :value="value" clearable @change="_selectChange"  placeholder="请选择">
   <template  v-for="item in categorydata">
 <el-option
      :key="item.key"
      :label="item.key"
      :value="item.val">
    </el-option>
</template>
  </el-select>`,
    props: ["value"],
    data: function () {
        return {
            categorydata: [
                { key: "video/mp4", val: 0 },
                { key: "video/x-flv", val: 1 },
                { key: "rtmp/flv", val: 2 },
                { key: "application/x-mpegURL", val: 3 }]
        };
    },
    methods: {
        _selectChange: function (value) {
            var formattedValue = value
            // 通过 input 事件带出数值
            this.$emit('input', formattedValue)
        }
    }
});


Vue.component("play-type-text", {
    props: ["state"], //{state:''},
    template: '<span v-html="obj[state]?obj[state].text:obj.d"></span>',
    data: function () {
        return {
            obj: {
                0: { text: 'video/mp4' },
                1: { text: 'video/x-flv' },
                2: { text: 'rtmp/flv' },
                3: { text: 'application/x-mpegURL' },
                d: ''
            }
        };
    }
});


Vue.component("livechannel-select", {
    template: `<el-select :value="value" clearable @change="_selectChange"  placeholder="请选择">
   <template  v-for="item in livechannel">
 <el-option
      :key="item.name"
      :label="item.name"
      :value="item.id">
    </el-option>
</template>
  </el-select>`,
    props: ["value"],
    data: function () {
        return {
            livechannel: []
        };
    },
    methods: {
        _selectChange: function (value) {
            var formattedValue = value
            // 通过 input 事件带出数值
            this.$emit('input', formattedValue)
        },
        getlivechannel: function () {
            var _soce = this;
            $.ajax({
                url: "/LiveChannel/GetLiveChannels",
                type: "post",
                success: function (res) {
                    _soce.livechannel = res;
                }
            });
        }
    },
    created: function () {
        this.getlivechannel();
    }
});




Vue.component("errvideo-select", {
    template: `<el-select :value="value" clearable @change="_selectChange"  placeholder="请选择">
   <template  v-for="item in videoData.data">
 <el-option
      :key="item.name"
      :label="item.name"
      :value="item.name">{{item.name}} &nbsp;
<a :href="item.url" target="_blank">查看</a>
    </el-option>
</template>
  </el-select>`,
    props: ["value"],
    data: function () {
        return {
            videoData: []
        };
    },
    methods: {
        _selectChange: function (value) {
            var formattedValue = value
            // 通过 input 事件带出数值
            this.$emit('input', formattedValue)
        },
        getErrVideo: function () {
            var _soce = this;
            $.ajax({
                url: "/Video/GetVideo",
                type: "post",
                success: function (res) {
                    _soce.videoData = res;
                }
            });
        }
    },
    created: function () {
        this.getErrVideo();
    }
});


Vue.component("pull-type-text", {
    props: ["state"], //{state:''},
    template: '<span v-html="obj[state]?obj[state].text:obj.d"></span>',
    data: function () {
        return {
            obj: {
                0: { text: 'video/mp4' },
                1: { text: 'video/x-flv' },
                2: { text: 'rtmp/flv' },
                3: { text: 'application/x-mpegURL' },
                d: ''
            }
        };
    }
});


Vue.component("live-type-text", {
    props: ["state"], //{state:''},
    template: '<span v-html="obj[state]?obj[state].text:obj.d"></span>',
    data: function () {
        return {
            obj: {
                2: { text: '播放中' },
                4: { text: '暂停中' },
                3: { text: '异常' },
                d: ''
            }
        };
    }
});



Vue.component("pull-type", {
    template: `<el-select :value="value" clearable @change="_selectChange"  placeholder="请选择">
   <template  v-for="item in categorydata">
 <el-option
      :key="item.key"
      :label="item.key"
      :value="item.val">
    </el-option>
</template>
  </el-select>`,
    props: ["value"],
    data: function () {
        return {
            categorydata: [
                { key: "监控", val: 0 }
            ]
        };
    },
    methods: {
        _selectChange: function (value) {
            var formattedValue = value
            // 通过 input 事件带出数值
            this.$emit('input', formattedValue)
        }
    }
});

Vue.component("pull-type-text", {
    props: ["state"], //{state:''},
    template: '<span v-html="obj[state]?obj[state].text:obj.d"></span>',
    data: function () {
        return {
            obj: {
                0: { text: '监控' },
                d: ''
            }
        };
    }
});