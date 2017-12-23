// 待导入的组件
import { NavigationBarComponent } from './Navigation-Bar/Navigation-Bar.component'

// 导出单个组件，方便路由使用
export {
  NavigationBarComponent,
};

// 导出所有页面，方便在module中一次性注入
export const ALL_COMPONENTS = [
  NavigationBarComponent
];