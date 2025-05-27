-- Sample Recipes Data
-- Make sure to run this after the main db.sql script

USE RecipeDB;

-- Insert sample recipes
INSERT INTO Recipes (Title, Description, Instructions, PrepTimeMinutes, CookTimeMinutes, Servings, Difficulty, ImageUrl, UserId, CategoryId) VALUES
-- Breakfast Recipes
('Classic Pancakes', 'Fluffy and delicious pancakes perfect for weekend mornings', 
'1. Mix dry ingredients in a large bowl\n2. In another bowl, whisk together milk, eggs, and melted butter\n3. Pour wet ingredients into dry ingredients and stir until just combined\n4. Heat griddle over medium heat\n5. Pour 1/4 cup batter for each pancake\n6. Cook until bubbles form on surface, then flip\n7. Cook until golden brown on both sides\n8. Serve hot with syrup', 
10, 15, 4, 'Easy', 'https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445', 1, 1),

('Avocado Toast', 'Simple and nutritious breakfast with creamy avocado', 
'1. Toast bread slices until golden\n2. Mash avocado with lime juice, salt, and pepper\n3. Spread avocado mixture on toast\n4. Top with cherry tomatoes and red pepper flakes\n5. Drizzle with olive oil\n6. Serve immediately', 
5, 5, 2, 'Easy', 'https://images.unsplash.com/photo-1541519227354-08fa5d50c44d', 1, 1),

-- Lunch Recipes
('Caesar Salad', 'Classic Caesar salad with homemade dressing', 
'1. Make dressing by whisking together mayo, parmesan, lemon juice, garlic, and anchovies\n2. Wash and chop romaine lettuce\n3. Toss lettuce with dressing\n4. Add croutons and extra parmesan\n5. Serve immediately', 
15, 0, 4, 'Medium', 'https://images.unsplash.com/photo-1546793665-c74683f339c1', 1, 2),

('Grilled Chicken Sandwich', 'Juicy grilled chicken with fresh vegetables', 
'1. Season chicken breast with salt, pepper, and herbs\n2. Grill chicken for 6-7 minutes per side\n3. Toast buns lightly\n4. Assemble sandwich with lettuce, tomato, and mayo\n5. Serve with side salad or fries', 
10, 15, 2, 'Easy', 'https://images.unsplash.com/photo-1553979459-d2229ba7433a', 1, 2),

-- Dinner Recipes
('Spaghetti Carbonara', 'Creamy Italian pasta with eggs and pancetta', 
'1. Cook spaghetti according to package directions\n2. Fry pancetta until crispy\n3. Whisk eggs with parmesan and black pepper\n4. Drain pasta and immediately toss with egg mixture\n5. Add pancetta and toss quickly\n6. Serve immediately with extra parmesan', 
10, 15, 4, 'Medium', 'https://images.unsplash.com/photo-1621996346565-e3dbc353d2e5', 1, 3),

('Beef Stir Fry', 'Quick and healthy stir fry with fresh vegetables', 
'1. Slice beef thinly against the grain\n2. Heat oil in wok or large pan\n3. Stir fry beef for 2-3 minutes\n4. Add vegetables and stir fry for 3-4 minutes\n5. Add sauce and toss to combine\n6. Serve over rice', 
15, 10, 4, 'Easy', 'https://images.unsplash.com/photo-1603133872878-684f208fb84b', 1, 3),

-- Dessert Recipes
('Chocolate Chip Cookies', 'Classic homemade chocolate chip cookies', 
'1. Cream butter and sugars together\n2. Beat in eggs and vanilla\n3. Mix in flour, baking soda, and salt\n4. Fold in chocolate chips\n5. Drop spoonfuls on baking sheet\n6. Bake at 375°F for 9-11 minutes\n7. Cool on wire rack', 
15, 10, 24, 'Easy', 'https://images.unsplash.com/photo-1499636136210-6f4ee915583e', 1, 4),

('Tiramisu', 'Classic Italian coffee-flavored dessert', 
'1. Make strong coffee and let cool\n2. Whisk egg yolks with sugar until thick\n3. Add mascarpone and mix until smooth\n4. Whip cream to soft peaks and fold in\n5. Dip ladyfingers in coffee and layer in dish\n6. Spread mascarpone mixture over cookies\n7. Repeat layers and chill overnight\n8. Dust with cocoa before serving', 
30, 0, 8, 'Hard', 'https://images.unsplash.com/photo-1571877227200-a0d98ea607e9', 1, 4),

-- Snack Recipes
('Hummus', 'Creamy chickpea dip perfect for vegetables', 
'1. Drain and rinse chickpeas\n2. Blend chickpeas with tahini, lemon juice, and garlic\n3. Add olive oil gradually while blending\n4. Season with salt and cumin\n5. Garnish with paprika and olive oil\n6. Serve with pita or vegetables', 
10, 0, 6, 'Easy', 'https://images.unsplash.com/photo-1571197119282-7c4e99e6e3d6', 1, 5),

('Energy Balls', 'No-bake protein-packed snack balls', 
'1. Mix oats, protein powder, and chia seeds\n2. Add peanut butter and honey\n3. Mix until well combined\n4. Roll into small balls\n5. Chill for 30 minutes to firm up\n6. Store in refrigerator', 
15, 0, 12, 'Easy', 'https://images.unsplash.com/photo-1606313564200-e75d5e30476c', 1, 5);

-- Insert recipe ingredients
INSERT INTO RecipeIngredients (RecipeId, Name, Quantity) VALUES
-- Pancakes ingredients
(1, 'All-purpose flour', '2 cups'),
(1, 'Sugar', '2 tablespoons'),
(1, 'Baking powder', '2 teaspoons'),
(1, 'Salt', '1 teaspoon'),
(1, 'Milk', '1 3/4 cups'),
(1, 'Eggs', '2 large'),
(1, 'Melted butter', '1/4 cup'),

-- Avocado Toast ingredients
(2, 'Bread slices', '4 pieces'),
(2, 'Ripe avocados', '2 large'),
(2, 'Lime juice', '2 tablespoons'),
(2, 'Cherry tomatoes', '1 cup'),
(2, 'Red pepper flakes', '1/4 teaspoon'),
(2, 'Olive oil', '2 tablespoons'),
(2, 'Salt', 'to taste'),
(2, 'Black pepper', 'to taste'),

-- Caesar Salad ingredients
(3, 'Romaine lettuce', '2 heads'),
(3, 'Mayonnaise', '1/2 cup'),
(3, 'Parmesan cheese', '1/2 cup grated'),
(3, 'Lemon juice', '2 tablespoons'),
(3, 'Garlic', '2 cloves minced'),
(3, 'Anchovy fillets', '4 pieces'),
(3, 'Croutons', '1 cup'),

-- Grilled Chicken Sandwich ingredients
(4, 'Chicken breast', '2 pieces'),
(4, 'Hamburger buns', '2 pieces'),
(4, 'Lettuce', '4 leaves'),
(4, 'Tomato', '1 large sliced'),
(4, 'Mayonnaise', '2 tablespoons'),
(4, 'Salt', 'to taste'),
(4, 'Black pepper', 'to taste'),
(4, 'Italian herbs', '1 teaspoon'),

-- Spaghetti Carbonara ingredients
(5, 'Spaghetti', '1 pound'),
(5, 'Pancetta', '6 oz diced'),
(5, 'Eggs', '4 large'),
(5, 'Parmesan cheese', '1 cup grated'),
(5, 'Black pepper', '1 teaspoon'),
(5, 'Salt', 'to taste'),

-- Beef Stir Fry ingredients
(6, 'Beef sirloin', '1 pound sliced'),
(6, 'Bell peppers', '2 cups sliced'),
(6, 'Broccoli', '2 cups florets'),
(6, 'Carrots', '1 cup sliced'),
(6, 'Soy sauce', '3 tablespoons'),
(6, 'Garlic', '3 cloves minced'),
(6, 'Ginger', '1 tablespoon minced'),
(6, 'Vegetable oil', '2 tablespoons'),

-- Chocolate Chip Cookies ingredients
(7, 'Butter', '1 cup softened'),
(7, 'Brown sugar', '3/4 cup'),
(7, 'White sugar', '1/4 cup'),
(7, 'Eggs', '2 large'),
(7, 'Vanilla extract', '2 teaspoons'),
(7, 'All-purpose flour', '2 1/4 cups'),
(7, 'Baking soda', '1 teaspoon'),
(7, 'Salt', '1 teaspoon'),
(7, 'Chocolate chips', '2 cups'),

-- Tiramisu ingredients
(8, 'Ladyfinger cookies', '24 pieces'),
(8, 'Strong coffee', '1 1/2 cups cooled'),
(8, 'Egg yolks', '6 large'),
(8, 'Sugar', '3/4 cup'),
(8, 'Mascarpone cheese', '1 1/4 pounds'),
(8, 'Heavy cream', '1 3/4 cups'),
(8, 'Cocoa powder', 'for dusting'),

-- Hummus ingredients
(9, 'Chickpeas', '1 can (15 oz)'),
(9, 'Tahini', '1/4 cup'),
(9, 'Lemon juice', '3 tablespoons'),
(9, 'Garlic', '2 cloves'),
(9, 'Olive oil', '3 tablespoons'),
(9, 'Cumin', '1/2 teaspoon'),
(9, 'Salt', 'to taste'),
(9, 'Paprika', 'for garnish'),

-- Energy Balls ingredients
(10, 'Rolled oats', '1 cup'),
(10, 'Protein powder', '2 scoops'),
(10, 'Chia seeds', '2 tablespoons'),
(10, 'Peanut butter', '1/2 cup'),
(10, 'Honey', '1/3 cup'),
(10, 'Mini chocolate chips', '1/4 cup');

-- Insert some tags
INSERT INTO Tags (Name) VALUES
('Vegetarian'),
('Vegan'),
('Gluten-Free'),
('Dairy-Free'),
('High-Protein'),
('Low-Carb'),
('Quick'),
('Healthy'),
('Comfort Food'),
('Italian'),
('Asian'),
('Mexican'),
('Mediterranean'),
('Breakfast'),
('Lunch'),
('Dinner'),
('Dessert'),
('Snack');

-- Insert recipe tags (many-to-many relationships)
INSERT INTO RecipeTags (RecipeId, TagId) VALUES
-- Pancakes
(1, 1), (1, 9), (1, 14),
-- Avocado Toast
(2, 1), (2, 7), (2, 8), (2, 14),
-- Caesar Salad
(3, 7), (3, 15),
-- Grilled Chicken Sandwich
(4, 5), (4, 15),
-- Spaghetti Carbonara
(5, 10), (5, 9), (5, 16),
-- Beef Stir Fry
(6, 11), (6, 5), (6, 7), (6, 16),
-- Chocolate Chip Cookies
(7, 1), (7, 9), (7, 17),
-- Tiramisu
(8, 1), (8, 10), (8, 17),
-- Hummus
(9, 1), (9, 2), (9, 8), (9, 13), (9, 18),
-- Energy Balls
(10, 1), (10, 5), (10, 8), (10, 18); 