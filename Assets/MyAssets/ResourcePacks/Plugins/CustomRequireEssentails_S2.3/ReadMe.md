# T2.3

### 应用场景: 
- 当你的组件依赖某些引用项, 否则无法运行时, 可以使用此功能在Inspector面板强调必须组件
### 如何使用: 
- #### 添加额外变量并添加请求必须项标记
1. 使用RequireEssentials字段标记
    - ```
        [RequireEssentials(typeof(SpriteRenderer), typeof(CircleCollider2D))]
        public String RequireEssentials;
      ```
    - 优点: 
      - 可以对一个字段声明多个必须组件
      - 可以声明所有组件类型
      - 使用一个额外的变量并标记所有你请求的组件即可
    - 缺点: 
      - 已知一个bug: 使用后的脚本内没有回调方法面板启用复选框会消失
      - 现在只能检测与请求Gameobject自身组件

### 更新信息: 
更新了命名空间改为gstools