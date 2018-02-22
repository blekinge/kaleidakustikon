<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:mei="http://www.music-encoding.org/ns/mei"
    xmlns="http://www.music-encoding.org/ns/mei" exclude-result-prefixes="mei">
    <xsl:output method="xml" version="1.0" indent="yes"/>
    <xsl:strip-space elements="*"/>
    
    <!-- If section is b. Add flats -->
    <xsl:template priority='2' match="mei:section[@type = 'B']">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*" mode="minor"/>
        </xsl:copy>
    </xsl:template>
    
    <!-- Add flat if b, a or d -->
    <xsl:template match="mei:note/@pname[.='b' or .='a' or .='d']" mode="minor">
        <xsl:attribute name="pname">
            <xsl:value-of select="."/>
        </xsl:attribute>
        <xsl:if test="../not(@accid)">
            <xsl:attribute name="accid.ges">
                <xsl:value-of select="'f'"/>
            </xsl:attribute>
        </xsl:if>
    </xsl:template>
    
    <xsl:template match="node()|@*" mode="minor">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*" mode="minor"/>
        </xsl:copy>
    </xsl:template>
    
    <xsl:template priority="1" match="node()|@*">
        <xsl:copy>
            <xsl:apply-templates select="node()|@*"/>
        </xsl:copy>
    </xsl:template>
    
</xsl:stylesheet>