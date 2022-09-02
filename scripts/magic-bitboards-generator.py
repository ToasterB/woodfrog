import numpy

outfile = open('bishopPattern.txt', 'w+')
i = 0

for row in range(0,8):
    for col in range(0,8):
        output = numpy.zeros(64, dtype='int')