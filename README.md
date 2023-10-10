# HotelCompare
An Azure OpenAI powered Hotel Review Comparison tool.

# Goal
to automate the hotel comparison process based on their reviews on booking.com (and/or the likes)

# Technology 
- .Net 7
- Azure Cognitive Search
- Semantic Kernel
- Azure OpenAI

# Input
- Up to 5 hotels to compare
- User’s preference (i.e. I want a hotel with good location, close to city center, cheap, good value, good food, etc)

# Output 
Each hotel reviews will be analysed and the app will come up with the followings:
- popular sentiment from the whole reviews
- what feature of the hotel that the guests like the most
- what feature of the hotel that the guests dislike the most
- summary of the whole reviews

The app then will compate the analysis results of the hotels and pick the top 2 hotels based on the user preference matrix.\

# Project Structure
