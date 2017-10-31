<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="@* | node()">
    <table width="950" cellpadding="0" cellspacing="0">
      <xsl:for-each select="ItemList/ArrayOfCustomItem">
        <tr style="height:310px;">
          <xsl:for-each select="CustomItem">
            <td width="230">
              <table cellpadding="0" cellspacing="0" style="text-align: center;font-family:'Microsoft YaHei';">
                <tr>
                  <td>
                    <a style="cursor:pointer" href="{OuterId}"><img src="{FivePicUrl}" width="230" height="230" style="border:0px;"/></a>
                  </td>
                </tr>
                <tr style="height:30px;">
                  <td>
                    <div style="font-size:14px;color:#000000;width: 208px;margin-left:11px;display: block;word-break: keep-all;white-space: nowrap;overflow: hidden;">
                      <xsl:value-of select="Title"/>
                    </div>
                  </td>
                </tr>
                <tr style="height:30px;">
                  <td style="font-size:18px;font-weight:bold;color:#ff5b5a">
                    <span style="font-size:10px;font-weight:bold">￥</span>
                    <xsl:value-of select="NewPrice"/>
                  </td>
                </tr>
                <tr style="height:33px;display:none">
                  <td style="background: #ff5b5a;font-size:12px;color:#ffffff;">
                    <a style="text-decoration:none;color:#fff;cursor:pointer" href="{OuterId}">立即抢购</a>
                  </td>
                </tr>
              </table>
            </td>
            <xsl:if test="(SerialNo mod 4) != 0">
              <td width="10"></td>
            </xsl:if>
          </xsl:for-each>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>
</xsl:stylesheet>
