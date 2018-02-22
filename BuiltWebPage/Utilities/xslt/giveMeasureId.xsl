<!-- 
    Takes an MEI file and adds a unique id to each measure. Handy for midi playback with note highlights.
    
    by Jonas Mortensen
-->
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:mei="http://www.music-encoding.org/ns/mei"
    xmlns="http://www.music-encoding.org/ns/mei" exclude-result-prefixes="mei">
    <xsl:template match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match="mei:measure">
        <measure>
            <xsl:attribute name="id">
                <xsl:value-of select="generate-id(.)"/>
            </xsl:attribute>
            
            <xsl:apply-templates select="@*|node()"/>
        </measure>
    </xsl:template>
</xsl:stylesheet>