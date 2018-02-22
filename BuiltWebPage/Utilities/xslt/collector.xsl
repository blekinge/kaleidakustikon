<?xml version="1.0" encoding="UTF-8"?>
<!--
    Takes an input string as a parameter and collects all 'cards' from external files based on the input and merges them to one file form the noMusic.mei file
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:mei="http://www.music-encoding.org/ns/mei" xmlns="http://www.music-encoding.org/ns/mei"
    xmlns:xlink="http://www.w3.org/1999/xlink" exclude-result-prefixes="" version="2.0">

    <xsl:output method="xml" indent="yes" encoding="UTF-8" omit-xml-declaration="no" standalone="no"/>
    <xsl:strip-space elements="*"/>

    <!-- The parameter holding the combination of cards as a single string -->
    <xsl:param name="comb"/>

    <xsl:template match="mei:section">
        <!-- Call recursive template -->
        <xsl:call-template name="parseInput"/>
    </xsl:template>

    <!-- This template is run one time for each character in the comb strings -->
    <xsl:template name="parseInput">
        <!-- two parameters to determine if and how many times the template has run -->
        <xsl:param name="index" select="1"/>
        <xsl:param name="total" select="string-length($comb)"/>
        
        <xsl:variable name="tempCardId" select="substring($comb, $index, 1)"/>
        
        <xsl:variable name="cardId">
            <xsl:choose>
                <xsl:when test="$tempCardId = 'a'">
                    <xsl:text>10</xsl:text>
                </xsl:when>
                <xsl:when test="$tempCardId = 'b'">
                    <xsl:text>11</xsl:text>
                </xsl:when>
                <xsl:when test="$tempCardId = 'c'">
                    <xsl:text>12</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="$tempCardId"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>

        <!-- Get the card id at the index in the comb string-->
        <!-- <xsl:variable name="cardId" select="substring($comb, $index, 1)"/> -->
        
        <!-- interpret the index as a card group character (could possibly be done smarter)  -->
        <xsl:variable name="cardGroup">
            <xsl:choose>
                <xsl:when test="$index = 1">
                    <xsl:text>a</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 2">
                    <xsl:text>b</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 3">
                    <xsl:text>c</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 4">
                    <xsl:text>d</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 5">
                    <xsl:text>e</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 6">
                    <xsl:text>f</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 7">
                    <xsl:text>g</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 8">
                    <xsl:text>h</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 9">
                    <xsl:text>i</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 10">
                    <xsl:text>k</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 11">
                    <xsl:text>l</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 12">
                    <xsl:text>m</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 13">
                    <xsl:text>n</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 14">
                    <xsl:text>o</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 15">
                    <xsl:text>p</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 16">
                    <xsl:text>q</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 17">
                    <xsl:text>r</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 18">
                    <xsl:text>s</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 19">
                    <xsl:text>t</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 20">
                    <xsl:text>u</xsl:text>
                </xsl:when>
                <xsl:when test="$index = 21">
                    <xsl:text>v</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:text>invalid</xsl:text>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        
        <!-- Get the file containing the cardNo -->
        <xsl:variable name="cardNumFile"
            select="document(concat('../mei/Cards/kaleidakustikon_', $cardId, '.mei'))"/>
        
        <!-- pass a cardcode (e.g. 'a.2') to the templates that gets the correct card -->
        <xsl:apply-templates select="$cardNumFile" mode="getCard">
            <xsl:with-param name="cardCode">
                <xsl:value-of select="concat($cardGroup, '.', $cardId)"/>
            </xsl:with-param>
        </xsl:apply-templates>
        
        <!-- If the end of the input has not been reached -->
        <xsl:if test="not($index = $total)">
            <!-- call template with incremented index -->
            <xsl:call-template name="parseInput">
                <xsl:with-param name="index" select="$index + 1"/>
            </xsl:call-template>
        </xsl:if>
    </xsl:template>

    <!-- runs through card file. And passes on the card code -->
    <xsl:template match="@*|node()" mode="getCard">
        <xsl:param name="cardCode"/>
        <xsl:apply-templates select="@*|node()" mode="getCard">
            <xsl:with-param name="cardCode">
                <xsl:value-of select="$cardCode"/>
            </xsl:with-param>
        </xsl:apply-templates>
    </xsl:template>
    
    <!-- sees if the section corresponds the the correct card. If so; copy -->
    <xsl:template match="mei:section" mode="getCard">
        <xsl:param name="cardCode"/>
        <xsl:if test="@xml:id = $cardCode">
            <xsl:copy>
                <xsl:copy-of select="@*|node()"/>
                <!-- <xsl:apply-templates select="descendant::*"/> -->
            </xsl:copy>
        </xsl:if>
    </xsl:template>

    <xsl:template match="@*|node()">
        <xsl:copy>
            <xsl:apply-templates select="@*|node()"/>
        </xsl:copy>
    </xsl:template>

</xsl:stylesheet>
