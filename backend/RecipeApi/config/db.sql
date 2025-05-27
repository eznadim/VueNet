-- Create database
CREATE DATABASE RecipeDB;
USE RecipeDB;

-- Users table
CREATE TABLE Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Categories table
CREATE TABLE Categories (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Recipes table
CREATE TABLE Recipes (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Title VARCHAR(200) NOT NULL,
    Description TEXT,
    Instructions LONGTEXT NOT NULL,
    PrepTimeMinutes INT,
    CookTimeMinutes INT,
    Servings INT,
    Difficulty ENUM('Easy', 'Medium', 'Hard') DEFAULT 'Easy',
    ImageUrl VARCHAR(500),
    UserId INT NOT NULL,
    CategoryId INT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE SET NULL
);

-- Ingredients table
CREATE TABLE Ingredients (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(100) UNIQUE NOT NULL,
    Unit VARCHAR(20), -- cups, tablespoons, grams, etc.
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Recipe ingredients junction table
CREATE TABLE RecipeIngredients (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    RecipeId INT NOT NULL,
    IngredientId INT NOT NULL,
    Quantity DECIMAL(10,2) NOT NULL,
    Unit VARCHAR(20),
    Notes VARCHAR(200),
    FOREIGN KEY (RecipeId) REFERENCES Recipes(Id) ON DELETE CASCADE,
    FOREIGN KEY (IngredientId) REFERENCES Ingredients(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_recipe_ingredient (RecipeId, IngredientId)
);

-- Tags table
CREATE TABLE Tags (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(50) UNIQUE NOT NULL,
    Color VARCHAR(7) DEFAULT '#007bff', -- hex color code
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Recipe tags junction table
CREATE TABLE RecipeTags (
    RecipeId INT NOT NULL,
    TagId INT NOT NULL,
    PRIMARY KEY (RecipeId, TagId),
    FOREIGN KEY (RecipeId) REFERENCES Recipes(Id) ON DELETE CASCADE,
    FOREIGN KEY (TagId) REFERENCES Tags(Id) ON DELETE CASCADE
);

-- Recipe ratings table
CREATE TABLE RecipeRatings (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    RecipeId INT NOT NULL,
    UserId INT NOT NULL,
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    Review TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (RecipeId) REFERENCES Recipes(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_user_recipe_rating (RecipeId, UserId)
);

-- Insert sample categories
INSERT INTO Categories (Name, Description) VALUES
('Breakfast', 'Morning meals and brunch items'),
('Lunch', 'Midday meals and light dishes'),
('Dinner', 'Evening meals and main courses'),
('Desserts', 'Sweet treats and after-dinner delights'),
('Appetizers', 'Starters and small plates'),
('Beverages', 'Drinks and cocktails'),
('Salads', 'Fresh and healthy salad dishes'),
('Soups', 'Warm and comforting soup recipes');

-- Insert sample tags
INSERT INTO Tags (Name, Color) VALUES
('Vegetarian', '#28a745'),
('Vegan', '#20c997'),
('Gluten-Free', '#ffc107'),
('Quick & Easy', '#17a2b8'),
('Healthy', '#28a745'),
('Comfort Food', '#fd7e14'),
('Spicy', '#dc3545'),
('Low-Carb', '#6f42c1');

-- Insert sample ingredients
INSERT INTO Ingredients (Name, Unit) VALUES
('Flour', 'cups'),
('Sugar', 'cups'),
('Eggs', 'pieces'),
('Butter', 'tablespoons'),
('Salt', 'teaspoons'),
('Pepper', 'teaspoons'),
('Olive Oil', 'tablespoons'),
('Garlic', 'cloves'),
('Onion', 'pieces'),
('Tomatoes', 'pieces');

-- Create indexes for better performance
CREATE INDEX idx_recipes_user ON Recipes(UserId);
CREATE INDEX idx_recipes_category ON Recipes(CategoryId);
CREATE INDEX idx_recipes_created ON Recipes(CreatedAt);
CREATE INDEX idx_recipe_ingredients_recipe ON RecipeIngredients(RecipeId);
CREATE INDEX idx_recipe_ratings_recipe ON RecipeRatings(RecipeId);