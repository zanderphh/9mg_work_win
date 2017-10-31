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
						  <img src="{PicUrl}_240x240.jpg"/>
							<span>
							  <b><xsl:value-of select="NewPrice"/></b>
							</span>
						  </a>
					  </li>
					</xsl:for-each>
				</ul>
			</div>
		</div>
		
      
    </xsl:template>
</xsl:stylesheet>
