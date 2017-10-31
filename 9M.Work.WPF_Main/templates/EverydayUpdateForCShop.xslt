<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="@* | node()">

		<div class="mgEveryDayUpdate">
			<div class="goods_list">
				<h3><xsl:value-of select="DateString"/> 新品  布鲁格20款</h3>
				<ul>
					<xsl:for-each select="ItemList/CustomItem">
					  <li>
						<a href="{DetailUrl}" target="_blank">
						  <img class="top" src="{PicUrl}_270x270.jpg" width="266" height="266" />
						  <img class="down" src="{FivePicUrl}_270x270.jpg" width="266" height="266" />
						  <b class="bg"></b>
						  <span class="info">
							<span class="price">￥<xsl:value-of select="NewPrice"/></span>
							<span class="buy">点击购买</span>
						  </span>
						</a>
					  </li>
					</xsl:for-each>
				</ul>
			</div>
		</div>
		
      
    </xsl:template>
</xsl:stylesheet>
