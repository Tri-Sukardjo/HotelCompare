# HotelCompare
This is an Azure OpenAI powered Hotel Review Comparison backend API, written as a practice project related to Professional Development on Generative AI development.
The client application is stored in a different repository.

# Goal
To assist and expedite hotel comparison process by:
- Analysing and providing insights from a list of reviews for each of the selected hotels, and
- Providing recommendation on the preferred hotel based on user preferences against the review analysis results.

# Technologies 
- .Net 7
- Azure Language Service
- Azure OpenAI
- Azure Cognitive Search
- Semantic Kernel
  
# Concepts Used
- Some of the main Generative AI capabilities to summarise texts, infer topics, and analyse sentiments.
- Prompt Engineering
- Semantic Kernel
- Semantic Memory

# Input 
Phase 1:
- Up to 3 hotels to compare
- User preferences in a text format (i.e. I want a hotel with good location, close to city center, cheap, good value, good food, etc)

# Output 
Phase 1: Each hotel reviews will be analysed and the app will come up with the followings:
- popular sentiment from the whole reviews
- what feature of the hotel that the guests like the most
- what feature of the hotel that the guests dislike the most
- summary of the whole reviews

The application then will analyse results against the user preferences provided and pick the top 1 recommended hotels along with the reason why.

# Project Structure
HotelCompareWebAPI  
├── Controllers          
├── Data              *(Dummy sample of hotel reviews)*  
├── Extensions        *(Processes to load configuration, initiate semantic kernel, and register skills)*  
├── Model                     
├── Services                  
├── Skills            *(This folder is where the skills are stored, containing folders with a set of skill prompt and config)*   
├── Utilities          
├── Program.cs  
└── appsettings.json  

# How to Run
1. Enter the details of your Azure OpenAI (Key, Endpoint, and Deployment) as well as the configurations for Text Embedding and Azure Cognitive Service in appsettings.json.
2. Run the Web API
3. Execute the API by sending a REST GET request, i.e. 	` http://localhost:7133/compare?hotelNames=<hotel-names-separated-by-commas>&userPreferences=<user-preferences-as-text> `
