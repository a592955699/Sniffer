# Sniffer
C# 通用采集模块，此代码是参照以前公司覃海潮同事代码，自己模仿写的,谢谢他的无私贡献。
最近半年多开始搞JAVA代码，所以用这个来试试手，熟悉下.net。

该项目理论上支持无线层级（具体时间没试过，暂时都是测试列表-详细这种结构）;
该项目采集时，暂时没设置登录相关验证等功能。如果有需要，建议做成传入cookie 字符串的功能，通过 CookieContainer 保存相信息。再调整下采集 html 的方法调用（我这里不需要登录，所以没实现，有朋友实现的话，可以提交上来）

一、Configs 类关系
/Sniffer.Core/Models/Configs 目录下有3个类：
1.ConfigBase.cs 是基本配置类，包括 url 地址，请求方式，编码格式，插件配置，子页的配置等。
2.ListConfig.cs 这个是列表页的配置类，包括分页相关设置信息。包括 列表中的 url 采集设置，分页页码采集设置，采集页数设置等。
3.DetailConfig.cs 这个是详细页的配置类，包括采集详细页的正则规则等。注意：部分详细页的内容页有分页，所以详细页是一个特殊的列表页。
配置参考 /Sniffer.Core/PageConfigs/ShopConfig.xml 

二、插件事件
该功能主要是处理一些采集的通用流程处理，采集完毕后需要对采集完毕的数据做相关的处理操作。这个就需要通过实现 IPlug 插件来实现自己的代码。
IPlug 接口有下面几个方法：
/// <summary>
/// 根页面采集完毕
/// </summary>
/// <param name="page"></param>
void OnRootPageDoneEventHandler(PageBase page);
/// <summary>
/// 列表的所有分页的url采集完毕
/// </summary>
/// <param name="page"></param>
void OnListUrlPageDoneEventHandler(ListPage page);
/// <summary>
/// 详细页采集完毕
/// 详细页内容有分页，需要所有内容都采集完毕，才执行
/// <param name="page"></param>
/// </summary>
void OnDetailPageDoneEventHandler(DetailPage page);
/// <summary>
/// 表页采以及下面对应的详细页集完毕
/// </summary>
/// <param name="page"></param>
void OnListPageDoneEventHandler(ListPage page);
/// <summary>
/// 页面采集完毕
/// </summary>
/// <param name="page"></param>
void OnPageDoneEventHandler(PageBase page);

1.void OnRootPageDoneEventHandler(PageBase page);
这个方法是当整一条采集先全部采集完毕后，才执行。
例如我们采集某宝网下的某一个店铺的所有产品信息时，我们设置根采集页就是店铺地址产品列表页。
我们采集完所有产品列表页中的产品后，才执行。触发这个时，等价于这个店铺的采集代办已经完全处理完毕了。

2.void OnListUrlPageDoneEventHandler(ListPage page);
代码采集流程是先采集所有列表页中的url的地址，将采集完的 url的地址加入到队列。
然后再去一个个去处理队列的其他url采集。
此方法需要所有的带采集列表页中的 url 的地址都采集完毕了，再触发。

3.void OnDetailPageDoneEventHandler(DetailPage page);
详细页采集完毕，再触发（包括详细页下的子页也要采集完毕）

4.void OnListPageDoneEventHandler(ListPage page);
列表页，以及列表页中 url 对应的子页都采集完毕，再触发。

5.void OnPageDoneEventHandler(PageBase page);
当前页面采集完毕就触发。注意：当一个页面采集完毕后，至少触发 OnPageDoneEventHandler事件。
同时还有可能触发 OnDetailPageDoneEventHandler 和 OnListPageDoneEventHandler 其中之一。

配置文档：
<?xml version="1.0" encoding="utf-8" ?>
<PageConfigs>
  <PageConfig Root="true" Name="Shop列表页">
    <!--ConfigBase 开始-->
    <Title>商品列表页</Title>
    <Url><![CDATA[https://gz.17zwd.com/shop/21675.htm]]></Url>
    <Encoding>UTF-8</Encoding>
    <MethodType>GET</MethodType>
    <PageType>ListPage</PageType>
    <SubPageConfig>Shop详细页</SubPageConfig>
    <Plug>Sniffer.Shop.Plugs.ShopPlug,Sniffer.Shop</Plug>
    <!--ConfigBase 结束-->
    
    <!--ListConfig 开始-->
    <UrlItem>
      <RegexString><![CDATA[<a class="promote-shop-image"[\s]*href="([^"]*)"[\s]*title="([^"]*)"]]></RegexString>
      <TitleGroupIndex>2</TitleGroupIndex>
      <UrlGroupIndex>1</UrlGroupIndex>
      <UrlFomart>https://gz.17zwd.com{0}</UrlFomart>
    </UrlItem>
    <StartIndex>1</StartIndex>
    <Increment>1</Increment>
    <MaxPage>1</MaxPage>
    <PageCountSnifferItem>
      <RegexString><![CDATA[<div class="all-goods-num">共 <span>(\d+)</span> 件相关商品</div>]]></RegexString>
      <ValueGroupIndex>1</ValueGroupIndex>
    </PageCountSnifferItem>
    <!--ListConfig 结束-->
  </PageConfig>

  <PageConfig Name="Shop详细页">
    <!--ConfigBase 开始-->
    <Title>商品详细页</Title>
    <Encoding>UTF-8</Encoding>
    <MethodType>GET</MethodType>
    <PageType>DetailPage</PageType>
    <Plug>Sniffer.Shop.Plugs.ShopPlug,Sniffer.Shop</Plug>
    <!--ConfigBase 结束-->

    <!--DetailConfig 开始-->
    <IsList>false</IsList>
    <FieldItems>
      <FieldItem>
        <Name>店铺Id</Name>
        <DefaultValue></DefaultValue>
        <SnifferItem>
          <RegexString><![CDATA[<meta property="og:product:nick" content="name=.*; url=https://.*?/shop/(\d+).htm"/>]]></RegexString>
          <ValueGroupIndex>1</ValueGroupIndex>
        </SnifferItem>
      </FieldItem>
      <FieldItem>
        <Name>产品Id</Name>
        <DefaultValue></DefaultValue>
        <SnifferItem>
          <RegexString><![CDATA[<link rel="canonical" href="https://.*?/item\?GID=(\d+)" />]]></RegexString>
          <ValueGroupIndex>1</ValueGroupIndex>
        </SnifferItem>
      </FieldItem>
    </FieldItems>
    <!--DetailConfig 结束-->
  </PageConfig>
</PageConfigs>
