#fetches required Messier data and adds it to the database
param ($dbServerName, $dbName,$userID, $userPW)

$DBConnectionstring= "Server = $dbServerName; Database = $dbName; Integrated Security = false; User ID = $userID; Password= $userPW;"


$Url = "https://lguerriero.opendatasoft.com/api/records/1.0/search/?dataset=catalogue-de-messier&rows=110"

#connectie
$Connection = New-Object System.Data.SQLClient.SQLConnection
$Connection.ConnectionString =  $DBConnectionstring
$Connection.Open()

#command
$Command = New-Object System.Data.SQLClient.SQLCommand
$Command.Connection = $Connection
$Request = Invoke-WebRequest -Uri $Url

#deserialized object
$Messier = ConvertFrom-Json($Request.Content)
$records = $Messier.records
$Request = Invoke-WebRequest -Uri $Url


$createquery = "
    IF OBJECT_ID(N'dbo.MessierCatalog', N'U') IS NOT NULL  
        DROP TABLE MessierCatalog
    CREATE TABLE MessierCatalog
        (ID VARCHAR(4) PRIMARY KEY, LatinName NVARCHAR(512), EnglishName NVARCHAR(512), FrenchName NVARCHAR(512), Declination NVARCHAR(64), RightAscention NVARCHAR(64), Magnitude TINYINT,	ImageLink NVARCHAR(MAX))
"

$Command.CommandText = $createquery
$Command.ExecuteNonQuery()

foreach($record in $records){
    $frenchName = $record.fields.french_name_nom_francais
    $latinName = $record.fields.latin_name_nom_latin
    $magnitude = $record.fields.mag
    $ID = $record.fields.messier
    $enlishName = $record.fields.english_name_nom_en_anglais
    $declination = $record.fields.dec
    $rightAscention = $record.fields.ra
    $imageLink = $record.fields.image

    $insertquery="
  INSERT INTO dbo.MessierCatalog
      ([ID],[LatinName],[EnglishName],[FrenchName],[Declination],[RightAscention],[Magnitude],[ImageLink])
    VALUES
      ('$ID','$latinName','$englishName','$frenchName','$declination','$rightAscention','$magnitude','$imageLink')"
  $Command.CommandText = $insertquery
  $Command.ExecuteNonQuery()


}

$Connection.Close();
