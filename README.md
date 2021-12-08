# PWS Exporter

PWS Exporter exports data (backup) from Wunderground using its API. 

## Usage

    PwsExporter.exe <api-key> <station-id> <start-date> [<end-date>]

### Example

    PwsExporter.exe <your-api-key> IPRAGU100 2019-05-01 2021-11-22

## Output

Output files are saved in the following structure: `./out/<station-id>/<year>/<month>/<date>.json`

## Wunderground API key

You can generate an API key in [Wunderground settings](https://www.wunderground.com/member/api-keys).

### API limitation

> Unfortunately, Wunderground.com does not offer unrestricted access to the data of a PWS. API Keys are basically limited in use as follows:
> - A maximum of 1500 calls per day
> - A maximum of 30 calls per minute

https://mypws.de/knowledgebase.php?article=6

To prevent the limit from exceeded, the program calls the API only every 2200 milliseconds.
This means that one year's export will take approximately 15-20 minutes (365 days × 2-3 seconds).
Also you should not export more than 4 years per day (1500 calls / 365 days).
