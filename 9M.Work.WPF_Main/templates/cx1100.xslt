<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>
  <xsl:template match="@* | node()">
    <div class='chuxiao1100'>
      <ul>
        <xsl:for-each select="ItemList/CustomItem">
          <li>
            <a href="{DetailUrl}" target="_blank">
              <img src="{PicUrl}_350x350.jpg" />
            </a>
            <div class='title'>
              <xsl:value-of select="Title"/>
            </div>
            <div class='price'>
              <ul>
                <li>
                  <div style='margin-top:20px'>
                    <span class='number'>
                      <span class='symbol'>¥</span>
                      <xsl:value-of select="NewPrice"/>
                    </span>
                  </div>
                </li>
                <li>
                  <div style='margin-top:8px;margin-bottom:2px;margin-left:10px'>
                    <span class='ellipse'>两件包邮</span>
                  </div>
                  <div style='margin-bottom:2px;margin-bottom:8px;margin-left:10px' class='dline_price'>
                    吊牌价:¥<xsl:value-of select="OldPrice"/>
                  </div>
                </li>
              </ul>
            </div>
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>
</xsl:stylesheet>
