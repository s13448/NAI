"""
authors: Marcin Biedrzycki, Tomasz Sosiński
emails: s134468@pjwstk.edu.pl , s16216@pjwstk.edu.pl
task: airbag
"""

import numpy as np
import skfuzzy as fuzz
from skfuzzy import control as ctrl

# Create inputs with wages
speed = ctrl.Antecedent(np.arange(0, 101, 10), 'speed')
passenger_weight = ctrl.Antecedent(np.arange(0, 101, 10), 'passenger_weight')
collision_power = ctrl.Antecedent(np.arange(0, 11, 1), 'collision_power')
airbag_power = ctrl.Consequent(np.arange(0, 11, 1), 'airbag_power')

# Populate the universe
speed.automf(3)
passenger_weight.automf(3)
collision_power.automf(3)

# Triangular function generator
airbag_power['low'] = fuzz.trimf(airbag_power.universe, [0, 0, 2.5])
airbag_power['medium'] = fuzz.trimf(airbag_power.universe, [0, 2.5, 5.0])
airbag_power['high'] = fuzz.trimf(airbag_power.universe, [5.0, 10.0, 10.0])

# Visual representation of the Rule
speed['average'].view()

collision_power.view()
passenger_weight.view()
airbag_power.view()

# Create a rule based on inputs
rule1 = ctrl.Rule(speed['poor'] | collision_power['poor'] | passenger_weight['average'], airbag_power['low'])
rule2 = ctrl.Rule(collision_power['average'], airbag_power['medium'])
rule3 = ctrl.Rule(speed['good'] | collision_power['good'], airbag_power['high'])

rule1.view()

airbag_ctrl = ctrl.ControlSystem([rule1, rule2, rule3])

# Create simulation
airbag = ctrl.ControlSystemSimulation(airbag_ctrl)

# Set input values in simulation
airbag.input['speed'] = 150
airbag.input['collision_power'] = 9
airbag.input['passenger_weight'] = 84

# crunch the numbers
airbag.compute()

# print results
print(airbag.output['airbag_power'])
collision_power.view(sim=airbag)
