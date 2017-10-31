<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="@* | node()">

		<div class="activity20161207">

			<ul>
				<xsl:for-each select="ItemList/CustomItem">
					<li>
						<a href="{DetailUrl}" target="_blank" style="text-decoration:none">
							<img src="{PicUrl}_270x270.jpg" width="266" height="266"></img>
							<div style="background-image: url(https://img.alicdn.com/imgextra/i1/73900259/TB2abQVXOBnpuFjSZFzXXaSrpXa-73900259.png); width: 266px; height: 120px;">
								<ul>
									<li>
										<span class="sp1"><xsl:value-of select="DiffPrice"/></span>
										<span class="sp2"><xsl:value-of select="OldPrice"/></span>
									</li>
									<li>
										<span class="sp3"><xsl:value-of select="NewPrice"/></span>
										<span class="sp4">.00</span>
									</li>
								</ul>
							</div>
						</a>
					</li>
				</xsl:for-each>
			</ul>
		</div>
    </xsl:template>
</xsl:stylesheet>
