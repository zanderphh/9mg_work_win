<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="@* | node()">

    <div class="zhuanti1100">
      <ul>
        <xsl:for-each select="ItemList/CustomItem">
          <li>
            <a href="{DetailUrl}" target="_blank">
              <img src="{FivePicUrl}_270x270.jpg" width="266" height="266" />
              <div class="Gtitle">
                <xsl:value-of select="Title"/>
              </div>
              <div class="price">
                <span>￥</span>
                <xsl:value-of select="NewPrice"/>
              </div>
              <div class="buy">
                <span class="title"> 立即抢购 </span>
                <span class="symbol">></span>
              </div>
            </a>
          </li>
        </xsl:for-each>
      </ul>
    </div>


  </xsl:template>
</xsl:stylesheet>
