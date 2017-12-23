建议的目录结构如下：

```
  pages/ --放置页面代码
    page1/
      page1.component.html --页面HTML代码
      page1.component.ts --页面组件代码
      page1.component.css --页面CSS代码
      page1.service.ts -- 页面服务代码
    page2/
    index.ts -- 集中管理所有的页面文件
  components/
    index.ts -- 集中管理所有的公共组件，模块内公共组件
  pipes/
    index.ts -- 集中导出pipe，pipe放置目录
  resources/ -- 模块静态资源目录（不会被打包处理，会自动拷贝到发布目录，引入时，请注意使用相对目录，相对于模块的目录：如 resource/xx.jpg）
  services/
    index.ts -- 集中管理services，公共服务放置目录
  app.module.ts -- 定义模块
  app.routing.ts -- 定义路由
  app.ts -- 统一处理要导出的内容，公共样式也在这里导入
  index.ts -- 固定写法，定义满足newkit加载器的模块
```