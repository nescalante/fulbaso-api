# Fulbaso API

> Experimental WCF Service exposing REST API

## Place Service

### /list
  
- Uri: http://fulbasoapi.apphb.com/places/list
- Parameters:
  
  - init: rows to skip in the query
  - rows: count of rows to show
  - term: a search term for the query
  - lat: latitude to search from
  - lng: longitude to search from
  - players: count of players for the courts
  - locations: locations to search into
  - tags: tags for the courts
  - indoor: set if the courts are indoor
  - lighted: set if the courts are lighted

### /get

- Uri: http://fulbasoapi.apphb.com/places/get/:id
- Parameters:

  - id: the id of the place
